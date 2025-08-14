using Microsoft.Extensions.Logging;
using ShadowGaze.Core.Models;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.Messages;

public class CommandStartMessageProcessor(
    PublicBotProperties botProperties,
    ILogger<CommandStartMessageProcessor> logger)
    : BaseMessageProcessor(botProperties)
{

    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, context) =>
        type is UpdateTypes.Message && update.Message?.Text == "/start";
    public override async Task Process(Update update)
    {
        logger.LogTrace($"Processing update message fom {update.Message?.From?.Username}");
        var answerText = "ShadowGaze from BigBroTeam";
        await Api.SendMessageAsync(update.Message.Chat.Id, answerText, replyMarkup: GetKeyboard());
        
    }

    private InlineKeyboardMarkup GetKeyboard()
    {
        var builder = new InlineKeyboardBuilder();
        builder.AppendCallbackData("Получить ссылку", "main;get_links");
        builder.AppendCallbackData("Выбрать подписку", "main;subscriptions");
        builder.AppendRow();
        builder.AppendCallbackData("Мой баланс", "main;get_account");
        builder.AppendRow();
        builder.AppendCallbackData("Инструкции", "main;instructions");
        return new InlineKeyboardMarkup(builder); ;
    }
}