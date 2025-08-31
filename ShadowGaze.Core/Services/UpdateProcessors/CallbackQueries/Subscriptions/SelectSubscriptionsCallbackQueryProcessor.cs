using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Core.Models.SessionContexts;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.CallbackQueries.Subscriptions;

public class SelectSubscriptionsCallbackQueryProcessor(PublicBotProperties botProperties)
    : BaseUpdateProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, _) =>
        type is UpdateTypes.CallbackQuery &&
        (update?.CallbackQuery?.Data?.StartsWith($"{CallbackQueriesConstants.Subscriptions};get;") ?? false);

    public override async Task Process(Update update, SessionContext sessionContext)
    {
        var query = update.CallbackQuery!;
        await Bot.AnswerCallbackQueryAsync(new AnswerCallbackQueryArgs(query.Id));
        var subType = int.Parse(query.Data!.Split(";")[2]);
        await Bot.SendMessageAsync(query.Message!.Chat.Id, "Выбор подписки скоро будет доступен");
    }
}