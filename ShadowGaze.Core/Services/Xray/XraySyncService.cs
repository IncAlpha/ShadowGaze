using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ShadowGaze.Core.Models;
using ShadowGaze.Data.Models.Database;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;

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

public class NotificationService(PublicBotProperties botProperties, CustomersRepository customersRepository)
{
    private ITelegramBotClient BotClient { get; } = botProperties.Bot;
    public async Task SendNotification()
    {
        var customers = customersRepository
            .AsQueryable()
            .Where(c => c.ExpiryDate.Date == DateTime.Now.Date.AddDays(1)).ToList();
        await SendUserNotificationAsync(customers,
            "Доступный период заканчивается. Используте команду /start для взаимодействия с ботом");
        
        customers = customersRepository
            .AsQueryable()
            .Where(c => c.ExpiryDate.Date == DateTime.Now.Date.AddDays(4)).ToList();
        await SendUserNotificationAsync(customers,
            "Доступный период закончится через 3 дня. Используте команду /start для взаимодействия с ботом");

    }

    private async Task SendUserNotificationAsync(IEnumerable<Customer> customers, string text)
    {
        foreach (var customer in customers)
        {
            await BotClient.SendMessageAsync(customer.TelegramId, text);
            await Task.Delay(1000);
            await BotClient.SendMessageAsync(customer.TelegramId, text);
        }
    }
}