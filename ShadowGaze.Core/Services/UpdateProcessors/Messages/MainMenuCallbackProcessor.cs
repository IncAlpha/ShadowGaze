using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.UpdatingMessages;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.Messages;

public class MainMenuCallbackProcessor(PublicBotProperties botProperties) : BaseMainMenuProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, _) =>
        type is UpdateTypes.CallbackQuery && update.CallbackQuery is { Data: CallbackQueriesConstants.MainMenu };

    public override async Task Process(Update update)
    {
        var message = update.CallbackQuery!.Message!;
        var answerText = "ShadowGaze from BigBroTeam";
        var args = new EditMessageTextArgs(answerText)
        {
            MessageId = message.MessageId,
            ChatId = message.Chat.Id,
            ReplyMarkup = GetKeyboard(),
        };
        await Api.EditMessageTextAsync<Message>(args);
    }
}