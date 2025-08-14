using ShadowGaze.Core.Models;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.Messages;

public class MainMenuMessageProcessor(PublicBotProperties botProperties) : BaseMainMenuProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, _) =>
        type is UpdateTypes.Message && update.Message is { Text: "/start" };

    public override async Task Process(Update update)
    {
        var chatId = update.Message!.Chat.Id;
        var answerText = "ShadowGaze from BigBroTeam";
        var sendMessageArgs = new SendMessageArgs(chatId, answerText)
        {
            ReplyMarkup = GetKeyboard(),
        };
        await Api.SendMessageAsync(sendMessageArgs);
    }
}