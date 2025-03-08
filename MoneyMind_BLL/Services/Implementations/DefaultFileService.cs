using Microsoft.Extensions.Configuration;
using MoneyMind_BLL.DTOs.DataDefaults;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace MoneyMind_BLL.Services.Implementations
{
    public class DefaultFileService : IDefaultFileService
    {
        private readonly string fileName;
        private readonly IWalletTypeRepository walletTypeRepository;

        // Khởi tạo đường dẫn file từ cấu hình và inject IWalletTypeRepository để kiểm tra walletTypeId
        public DefaultFileService(IConfiguration configuration, IWalletTypeRepository walletTypeRepository)
        {
            fileName = configuration["DefaultFileName"] ?? "datadefaults.json";
            this.walletTypeRepository = walletTypeRepository;
        }

        public async Task<DataDefaults> GetDefaultDataAsync()
        {
            var jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            if (!File.Exists(jsonFilePath))
            {
                throw new FileNotFoundException("Not found file default.", jsonFilePath);
            }
            var jsonString = await File.ReadAllTextAsync(jsonFilePath);
            var dataDefaults = JsonSerializer.Deserialize<DataDefaults>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (dataDefaults?.WalletCategories != null)
            {
                foreach (var walletCategory in dataDefaults.WalletCategories)
                {
                    bool exists = await walletTypeRepository.ExistsAsync(walletCategory.WalletTypeId);
                    if (!exists)
                    {
                        throw new Exception($"WalletTypeId {walletCategory.WalletTypeId} in wallet category '{walletCategory.Name}' isn't existing.");
                    }
                }
            }

            return dataDefaults;
        }

        public async Task WriteDefaultDataAsync(DataDefaults data)
        {
            var jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            // Kiểm tra walletTypeId trước khi ghi file
            if (data?.WalletCategories != null)
            {
                foreach (var walletCategory in data.WalletCategories)
                {
                    bool exists = await walletTypeRepository.ExistsAsync(walletCategory.WalletTypeId);
                    if (!exists)
                    {
                        throw new Exception($"WalletTypeId {walletCategory.WalletTypeId} in wallet category '{walletCategory.Name}' isn't existing.");
                    }
                }
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var jsonString = JsonSerializer.Serialize(data, options);
            await File.WriteAllTextAsync(jsonFilePath, jsonString);
        }
    }
}
