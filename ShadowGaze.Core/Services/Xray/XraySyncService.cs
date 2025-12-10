using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ShadowGaze.Core.Services.Xray;

public class XraySyncService(ILogger<XraySyncService> logger, IServiceProvider provider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var nextHour = now.AddHours(1).Date.AddHours(now.AddHours(1).Hour); 
            var delay = nextHour - now;

            logger.LogInformation("Следующий запуск в: {time}, через {delay}", nextHour, delay);

            try
            {
                await Task.Delay(delay, stoppingToken);
                using var scope = provider.CreateScope();
                var xrayService = scope.ServiceProvider.GetRequiredService<XrayService>();
                await xrayService.SyncAsync();
            }
            catch (TaskCanceledException)
            {
                return;
            }
        }
    }
}