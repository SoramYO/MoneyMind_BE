using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Hosting;
using MoneyMind_BLL.DTOs.GoogleSheet;
using MoneyMind_BLL.DTOs.Transactions;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Implementations;
using MoneyMind_DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace MoneyMind_BLL.Services.Implementations
{
    public class GoogleSheetSyncService : IGoogleSheetSyncService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ITransactionTagRepository _transactionTagRepository;
        private readonly IMLService _mlService;
        private readonly ITransactionService _transactionService;
		private readonly ITransactionSyncLogRepository _syncLogRepository;
        private readonly string _contentRootPath;
        private readonly ILogger<GoogleSheetSyncService> _logger;
        private readonly INotificationService _notificationService;

        public GoogleSheetSyncService(
            ITransactionRepository transactionRepository,
            ITransactionTagRepository transactionTagRepository,
            IMLService mlService,
            ITransactionSyncLogRepository syncLogRepository,
            IHostEnvironment hostEnvironment,
            ITransactionService transactionService,
            ILogger<GoogleSheetSyncService> logger,
            INotificationService notificationService)
        {
            _transactionRepository = transactionRepository;
            _transactionTagRepository = transactionTagRepository;
            _mlService = mlService;
            _syncLogRepository = syncLogRepository;
            _contentRootPath = hostEnvironment.ContentRootPath;
			_transactionService = transactionService;
            _logger = logger;
            _notificationService = notificationService;
		}

        public async Task<SyncResult> SyncTransactionsFromSheet(GoogleSheetRequest request)
        {
            var syncLog = new TransactionSyncLog
            {
                UserId = request.UserId,
                Status = "InProcess",
                ErrorMessage = ""
            };
            var result = new SyncResult { NewTransactions = 0 };

            try
            {
                Console.WriteLine($"Starting sync process for sheet ID: {request.SheetId}, User ID: {request.UserId}");
                Console.WriteLine("Adding sync log...");
                await _syncLogRepository.InsertAsync(syncLog);

                // Initialize Google Sheets service
                GoogleCredential credential;
                var credentialsPath = Path.Combine(_contentRootPath, "credentials.json");
                
                Console.WriteLine($"Looking for credentials at: {credentialsPath}");
                if (!File.Exists(credentialsPath))
                {
                    Console.WriteLine("Credentials file not found!");
                    throw new FileNotFoundException("Google credentials file not found.", credentialsPath);
                }

                Console.WriteLine("Loading Google credentials...");
                using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream)
                        .CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);
                }

                Console.WriteLine("Initializing Google Sheets service...");
                var service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "MoneyMind"
                });

                // Lấy danh sách tên sheets
                var sheetNames = await GetSheetNames(service, request.SheetId);
                
                foreach (var sheetName in sheetNames)
                {
                    try
                    {
                        Console.WriteLine($"Processing sheet: {sheetName}");
                        string range = $"{sheetName}!A2:M";
                        var sheetsRequest = service.Spreadsheets.Values.Get(request.SheetId, range);
                        var response = await sheetsRequest.ExecuteAsync();
                        var rows = response.Values;

                        Console.WriteLine($"Found {rows?.Count ?? 0} rows in sheet");

                        if (rows != null && rows.Count > 0)
                        {
                            int processedRows = 0;
                            int newTransactions = 0;
                            int skippedTransactions = 0;

                            foreach (var row in rows)
                            {
                                try
                                {
                                    processedRows++;
                                    Console.WriteLine($"Processing row {processedRows}/{rows.Count}");
                                    
                                    // Map row data to MBBankSheetRow object
                                    var sheetRow = MapRowToMBBankSheetRow(row);
                                    
                                    if (sheetRow.Amount != null)
                                    {
                                        // Xóa dấu phân cách hàng nghìn (.) và thay thế dấu phẩy thập phân (,) bằng dấu chấm (.) nếu có
                                        string normalizedAmount = sheetRow.Amount.Replace(".", "").Replace(",", ".");
                                        
                                        Console.WriteLine($"Original amount: {sheetRow.Amount}");
                                        Console.WriteLine($"Normalized amount: {normalizedAmount}");
                                        
                                        if (decimal.TryParse(normalizedAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount))
                                        {
                                            // Lấy giá trị tuyệt đối của amount vì chúng ta chỉ quan tâm đến giá trị dương
                                            amount = Math.Abs(amount);
                                            Console.WriteLine($"Parsed amount: {amount}");

                                            Console.WriteLine($"Checking for existing transaction: {sheetRow.Description}, Amount: {amount}");
                                            // Check if transaction already exists
                                            var existingTransaction = await _transactionRepository.IsExistTransaction(
                                                sheetRow.Description,
                                                (double)amount
                                            );

                                            if (existingTransaction == null)
                                            {
                                                Console.WriteLine("Transaction is new, getting category classification...");
                                                var tag = await _mlService.ClassificationTag(
                                                    sheetRow.Description
                                                );

                                                if (tag != null)
                                                {
                                                    Console.WriteLine($"Category classified as: {tag.Name}");


                                                    var transaction = new TransactionRequest
													{
														Amount = (double)amount,
														Description = sheetRow.Description,
														TransactionDate = DateTime.ParseExact(
															sheetRow.TransactionDate,
															new string[] {
																"dd/MM/yyyy HH:mm:ss",
																"yyyy-MM-dd HH:mm:ss",
																"MM/dd/yyyy HH:mm:ss"
															},
															CultureInfo.InvariantCulture,
															DateTimeStyles.None),
														RecipientName = sheetRow.CounterAccountName,
													};

													await _transactionService.AddTransactionAsync(request.UserId, transaction);
													newTransactions++;
                                                    Console.WriteLine("New transaction added successfully");
                                                    result.NewTransactions++;
                                                    
                                                    // Send notification for new transaction
                                                    await _notificationService.SendNotificationToUser(
                                                        request.UserId,
                                                        $"Đã thêm giao dịch mới: {transaction.Description} với số tiền {transaction.Amount:N0} VNĐ"
                                                    );
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Failed to classify category for transaction");
                                                }
                                            }
                                            else
                                            {
                                                skippedTransactions++;
                                                Console.WriteLine("Transaction already exists, skipping");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Failed to parse amount");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Invalid amount format or null amount: {sheetRow.Amount}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error processing row: {ex.Message}");
                                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                                }
                            }

                            Console.WriteLine($"Sync Summary:");
                            Console.WriteLine($"Total rows processed: {processedRows}");
                            Console.WriteLine($"New transactions added: {newTransactions}");
                            Console.WriteLine($"Skipped transactions: {skippedTransactions}");
                        }
                        else
                        {
                            Console.WriteLine("No data found in sheet");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing sheet {SheetName}", sheetName);
                    }
                }

                syncLog.Status = "Success";
                await _syncLogRepository.UpdateAsync(syncLog);
                Console.WriteLine("Sync completed successfully");
                
                // Send final sync notification
                await _notificationService.SendNotificationToUser(
                    request.UserId,
                    $"Đồng bộ hoàn tất! Đã thêm {result.NewTransactions} giao dịch mới."
                );
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical error in sync process: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                syncLog.Status = "Failed";
                syncLog.ErrorMessage = ex.Message;
                await _syncLogRepository.UpdateAsync(syncLog);
                throw;
            }
        }

        private async Task<List<string>> GetSheetNames(SheetsService service, string spreadsheetId)
        {
            try
            {
                Console.WriteLine($"Getting sheet names for spreadsheet: {spreadsheetId}");
                var spreadsheet = await service.Spreadsheets.Get(spreadsheetId).ExecuteAsync();
                var sheetNames = spreadsheet.Sheets.Select(s => s.Properties.Title).ToList();
                Console.WriteLine($"Found {sheetNames.Count} sheets: {string.Join(", ", sheetNames)}");
                return sheetNames;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting sheet names: {ex.Message}");
                throw;
            }
        }

        private BankSheetRow MapRowToMBBankSheetRow(IList<object> row)
        {
            return new  BankSheetRow
            {
                Id = row.Count > 0 ? row[0]?.ToString() : "",
                Description = row.Count > 1 ? row[1]?.ToString() : "",
                Amount = row.Count > 2 ? row[2]?.ToString() : "0",
                TransactionDate = row.Count > 3 ? row[3]?.ToString() : "",
                AccountNumber = row.Count > 4 ? row[4]?.ToString() : "",
                ReferenceNumber = row.Count > 5 ? row[5]?.ToString() : "",
                Balance = row.Count > 6 ? row[6]?.ToString() : "",
                VirtualAccountNumber = row.Count > 7 ? row[7]?.ToString() : "",
                VirtualAccountName = row.Count > 8 ? row[8]?.ToString() : "",
                CounterAccountNumber = row.Count > 9 ? row[9]?.ToString() : "",
                CounterAccountName = row.Count > 10 ? row[10]?.ToString() : "",
                BankBinCode = row.Count > 11 ? row[11]?.ToString() : "",
                PaymentChannel = row.Count > 12 ? row[12]?.ToString() : ""
            };
        }

	}
} 