using ShadowGaze.Core.Models;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.UpdatingMessages;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.CallbackQuery.Subscription;

public class SubscriptionsCallbackQueryProcessor(PublicBotProperties botProperties)
    : BaseCallbackQueryProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, context) =>
        type is UpdateTypes.CallbackQuery && update.CallbackQuery?.Data == "main;subscriptions";
    public override async Task Process(Update update)
    {
        var query = update.CallbackQuery;
        await Api.AnswerCallbackQueryAsync(new AnswerCallbackQueryArgs(query.Id));
        
        var builder = new InlineKeyboardBuilder();
        builder.AppendCallbackData("1 Месяц - 200₽", $"subscription;1");
        builder.AppendCallbackData("3 Месяца - 550₽", $"subscription;2");
        builder.AppendRow();
        builder.AppendCallbackData("6 Месяцев - 1050₽", $"subscription;6");
        builder.AppendCallbackData("12 Месяцев - 2000₽", $"subscription;12");
        await Api.EditMessageTextAsync<Message>(
            new EditMessageTextArgs("Выберите длительность подписки")
            {
                ChatId = query.Message.Chat.Id,
                MessageId = query.Message.MessageId,
                ReplyMarkup = new InlineKeyboardMarkup(builder)
            });
    }
}