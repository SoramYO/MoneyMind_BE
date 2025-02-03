using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using MoneyMind_BLL.DTOs.GoogleSheet;
using MoneyMind_BLL.Services.Interfaces;


namespace MoneyMind_BLL.Services.BackgroundServices
{
    public class SheetSyncService : BackgroundService
    {
        private readonly ILogger<SheetSyncService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public SheetSyncService(
            ILogger<SheetSyncService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Starting sheet sync process at: {time}", DateTimeOffset.Now);
                    
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                        var sheetService = scope.ServiceProvider.GetRequiredService<ISheetService>();
                        var googleSheetSyncService = scope.ServiceProvider.GetRequiredService<IGoogleSheetSyncService>();

                        var users = await userManager.Users.ToListAsync();
                        foreach (var user in users)
                        {
                            try
                            {
                                var userSheets = await sheetService.GetUserSheetsAsync(Guid.Parse(user.Id));
                                
                                if (userSheets.Any())
                                {
                                    _logger.LogInformation("Processing sheets for user: {userId}", user.Id);
                                    
                                    foreach (var sheet in userSheets)
                                    {
                                        try
                                        {
                                            var request = new GoogleSheetRequest
                                            {
                                                SheetId = sheet.SheetId,
                                                UserId = Guid.Parse(user.Id)
                                            };

                                            await googleSheetSyncService.SyncTransactionsFromSheet(request);
                                            _logger.LogInformation("Successfully synced sheet {sheetId} for user {userId}", 
                                                sheet.SheetId, user.Id);
                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.LogError(ex, "Error syncing sheet {sheetId} for user {userId}", 
                                                sheet.SheetId, user.Id);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error processing user {userId}", user.Id);
                            }
                        }
                    }

                    _logger.LogInformation("Completed sheet sync process at: {time}", DateTimeOffset.Now);
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in sheet sync background service");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }
    }
} 