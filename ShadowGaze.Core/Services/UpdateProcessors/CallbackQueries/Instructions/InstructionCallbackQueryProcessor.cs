using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.CallbackQueries.Instructions;

public class InstructionCallbackQueryProcessor(PublicBotProperties botProperties) : BaseUpdateProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, _) =>
        type is UpdateTypes.CallbackQuery && update.CallbackQuery?.Data == CallbackQueriesConstants.Instructions;

    public override async Task Process(Update update)
    {
        await Api.AnswerCallbackQueryAsync(new AnswerCallbackQueryArgs(update.CallbackQuery!.Id));
        await Api.SendMessageAsync(update.CallbackQuery!.Message!.Chat.Id, "Инструкции скоро будут доступны");
    }
}