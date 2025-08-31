using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.SessionContexts;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.Messages.MainMenu;

public class MainMenuMessageProcessor(PublicBotProperties botProperties, CustomersRepository customersRepository)
    : BaseMainMenuProcessor(botProperties, customersRepository)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, _) =>
        type is UpdateTypes.Message && update.Message is { Text: "/start" };

    protected override async Task AnswerProcess(Update update, SessionContext sessionContext)
    {
        var chatId = update.Message!.Chat.Id;
        var answerText = GetAnswerText();
        var sendMessageArgs = new SendMessageArgs(chatId, answerText)
        {
            ReplyMarkup = GetKeyboard(),
        };
        await Bot.SendMessageAsync(sendMessageArgs);
    }

    protected override long GetUserId(Update update)
    {
        return update.Message!.From!.Id;
    }
}