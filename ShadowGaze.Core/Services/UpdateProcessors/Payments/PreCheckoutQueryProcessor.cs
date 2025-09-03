using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.SessionContexts;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.Payments;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.Payments;

public class PreCheckoutQueryProcessor (PublicBotProperties botProperties) : BaseUpdateProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, context) =>
        type is UpdateTypes.PreCheckoutQuery;
    public override async Task Process(Update update, SessionContext sessionContext)
    {
        var preCheckoutQuery = update.PreCheckoutQuery!;
        await Bot.AnswerPreCheckoutQueryAsync(preCheckoutQuery.Id, true);
    }
}