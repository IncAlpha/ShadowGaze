using ShadowGaze.Core.Models;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.CallbackQuery.Subscription;

public class SelectSubscriptionsCallbackQueryProcessor(PublicBotProperties botProperties)
    : BaseCallbackQueryProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, context) =>
        type is UpdateTypes.CallbackQuery && (update?.CallbackQuery?.Data?.StartsWith("subscription") ?? false);
    public override async Task Process(Update update)
    {
        await Api.AnswerCallbackQueryAsync(new AnswerCallbackQueryArgs(update.CallbackQuery.Id)); 
        var query = update.CallbackQuery;
        var subType = int.Parse(query.Data.Split(";")[1]);
        await Api.SendMessageAsync(query.Message.Chat.Id, "Выбор подписки скоро будет доступен");
    }
}