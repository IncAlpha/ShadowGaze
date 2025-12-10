using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Data.Models.Database;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;

namespace ShadowGaze.Core.UserNotification;

public class NotificationService(PublicBotProperties botProperties, CustomersRepository customersRepository)
{
    private ITelegramBotClient BotClient { get; } = botProperties.Bot;
    public async Task SendNotification()
    {
        var customers = customersRepository
            .AsQueryable()
            .Where(c => c.ExpiryDate.Date == DateTime.Now.Date.AddDays(4)).ToList();
        await SendUserNotificationAsync(customers,
            "Здравствуйте! Напоминаем, что срок вашей подписки на доступ к прокси скоро подходит к концу.\n" +
            "Мы хотим, чтобы вы продолжали пользоваться быстрым и стабильным подключением без перерывов, поэтому рекомендуем продлить доступ заранее.\n" +
            "Если появятся вопросы — мы всегда рядом.");
        
         customers = customersRepository
            .AsQueryable()
            .Where(c => c.ExpiryDate.Date == DateTime.Now.Date.AddDays(1)).ToList();
        await SendUserNotificationAsync(customers,
            "Сегодня истекает срок вашей подписки.\n" +
            "Чтобы избежать отключения и сохранить доступ к прокси, продлите подписку прямо сейчас.\n" +
            "Если вам требуется помощь или появились сложности — дайте знать, мы с удовольствием поможем");
        
        customers = customersRepository
            .AsQueryable()
            .Where(c => c.ExpiryDate.Date == DateTime.Now.Date.AddDays(-3)).ToList();
        await SendUserNotificationAsync(customers,
            "Похоже, ваша подписка уже завершилась, и доступ к прокси временно недоступен.\n" +
            "Мы будем рады вернуть вам полный функционал — продление занимает всего пару секунд.\n" +
            "Если вам нужно уточнить детали, задать вопросы или восстановить доступ — напишите нам.");
    }

    private async Task SendUserNotificationAsync(IEnumerable<Customer> customers, string text)
    {
        foreach (var customer in customers)
        {
            var messageArgs = new SendMessageArgs(customer.TelegramId, text)
            {
                ReplyMarkup = BuildMainMenuKeyboard()
            };
            await BotClient.SendMessageAsync(messageArgs);
            await Task.Delay(1000);
        }
    }
    
    private InlineKeyboardMarkup BuildMainMenuKeyboard()
    {
        return BuildKeyboard()
            .AppendCallbackData("Продлить", CallbackQueriesConstants.Subscriptions)
            .AppendRow()
            .AppendUrl("Связаться с нами", "https://t.me/shadowgazeproxy")
            .AppendRow()
            .AppendCallbackData("Главное меню", CallbackQueriesConstants.MainMenu)
            .Build();
    }
}