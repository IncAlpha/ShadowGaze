using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.UpdatingMessages;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.Messages;

public class MainMenuCallbackProcessor(PublicBotProperties botProperties, CustomersRepository customersRepository)
    : BaseMainMenuProcessor(botProperties, customersRepository)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, _) =>
        type is UpdateTypes.CallbackQuery && update.CallbackQuery is { Data: CallbackQueriesConstants.MainMenu };

    protected override long GetUserId(Update update)
    {
        return update.CallbackQuery!.From.Id;
    }

    protected override async Task AnswerProcess(Update update)
    {
        var message = update.CallbackQuery!.Message!;
        var answerText = GetAnswerText();
        var args = new EditMessageTextArgs(answerText)
        {
            MessageId = message.MessageId,
            ChatId = message.Chat.Id,
            ReplyMarkup = GetKeyboard(),
        };
        await Api.EditMessageTextAsync<Message>(args);
    }
}