using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Core.Models.SessionContexts;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.Messages.MainMenu;

public class MainMenuCallbackProcessor(
    PublicBotProperties botProperties,
    CustomersRepository customersRepository,
    BotSectionsRepository sectionsRepository
) : BaseMainMenuProcessor(botProperties, customersRepository, sectionsRepository)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, _) =>
        type is UpdateTypes.CallbackQuery && update.CallbackQuery is { Data: CallbackQueriesConstants.MainMenu };

    protected override long GetUserId(Update update)
    {
        return update.CallbackQuery!.From.Id;
    }

    protected override long GetChatId(Update update)
    {
        return update.CallbackQuery!.Message!.Chat.Id;
    }
}