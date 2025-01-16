using Microsoft.Extensions.Logging;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Repositories.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

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
            var now = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var nextRun = now.Date.AddDays(1).AddSeconds(-1); // 23:59:59
            var delay = nextRun - now;

            if (delay.TotalMilliseconds > 0)
            {
                await Task.Delay(delay, stoppingToken);
            }

            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var accountBankRepository = scope.ServiceProvider.GetRequiredService<IAccountBankRepository>();
                    var mbBankSyncService = scope.ServiceProvider.GetRequiredService<IMBBankSyncService>();

                    var accounts = await accountBankRepository.GetAllUserIds();
                    foreach (var account in accounts)
                    {
                        await mbBankSyncService.SyncTransactions(account.UserId);
                        _logger.LogInformation("MB Bank transaction sync completed for user {userId} at: {time}",
                            account.UserId, DateTimeOffset.Now);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while syncing MB Bank transactions");
            }
        }
    }
}