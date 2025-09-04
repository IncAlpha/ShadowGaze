using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.SessionContexts;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.Messages.MainMenu;

public class MainMenuMessageProcessor(
    PublicBotProperties botProperties,
    CustomersRepository customersRepository,
    BotSectionsRepository sectionsRepository
) : BaseMainMenuProcessor(botProperties, customersRepository, sectionsRepository)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, _) =>
        type is UpdateTypes.Message && update.Message is { Text: "/start" };

    protected override long GetUserId(Update update)
    {
        return update.Message!.From!.Id;
    }

    protected override long GetChatId(Update update)
    {
        return update.Message!.Chat.Id;
    }
}