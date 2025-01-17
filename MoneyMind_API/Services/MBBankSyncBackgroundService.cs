using Microsoft.Extensions.Logging;
using MoneyMind_BLL.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class MBBankSyncBackgroundService : BackgroundService
{
    private readonly ILogger<MBBankSyncBackgroundService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public MBBankSyncBackgroundService(
        ILogger<MBBankSyncBackgroundService> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SyncAllAccountsAsync(stoppingToken);
                _logger.LogInformation("MB Bank transaction sync completed at: {time}", DateTimeOffset.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while syncing MB Bank transactions");
            }

            // Delay 24 hours
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private async Task SyncAllAccountsAsync(CancellationToken stoppingToken)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var accountBankService = scope.ServiceProvider.GetRequiredService<IAccountBankService>();
            var mbBankSyncService = scope.ServiceProvider.GetRequiredService<IMBBankSyncService>();
            var users = userManager.Users.ToList();

            foreach (var user in users)
            {
                if (stoppingToken.IsCancellationRequested) break;

                try
                {
                    var accountBank = await accountBankService.GetAccoutBankByUserIdAsync(Guid.Parse(user.Id));
                    if (accountBank != null)
                    {
                        await mbBankSyncService.SyncTransactions(Guid.Parse(user.Id));
                        _logger.LogInformation("MB Bank transaction sync completed for user {userId} at: {time}",
                            user.Id, DateTimeOffset.Now);
                    }
                    else
                    {
                        _logger.LogWarning("No bank account found for user {userId}", user.Id);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error syncing transactions for user {userId}", user.Id);
                }
            }
        }
    }
}
