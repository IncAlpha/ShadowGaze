using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
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

    public override async Task Process(Update update)
    {
        var query = update.CallbackQuery!;
        await Api.AnswerCallbackQueryAsync(new AnswerCallbackQueryArgs(query.Id));
        var subType = int.Parse(query.Data!.Split(";")[2]);
        await Api.SendMessageAsync(query.Message!.Chat.Id, "Выбор подписки скоро будет доступен");
    }
}