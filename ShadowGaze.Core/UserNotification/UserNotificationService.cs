using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ShadowGaze.Core.Services.Xray;

namespace ShadowGaze.Core.UserNotification;

public class UserNotificationService(ILogger<XraySyncService> logger, IServiceProvider provider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var target = DateTime.Today.AddHours(17); 
        
            if (now > target)
                target = target.AddDays(1);
        
            var delay = target - now;
            await Task.Delay(delay, stoppingToken);
        
            using var scope = provider.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();
            await notificationService.SendNotification();
        }
    }
}