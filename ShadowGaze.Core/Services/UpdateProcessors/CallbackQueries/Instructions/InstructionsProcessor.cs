using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Data.Models;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.UpdatingMessages;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.CallbackQueries.Instructions;

public class InstructionsProcessor(PublicBotProperties botProperties) : BaseUpdateProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, _) =>
        type is UpdateTypes.CallbackQuery && update.CallbackQuery?.Data == CallbackQueriesConstants.Instructions;

    public static readonly Dictionary<Platforms, string> PlatformButtons = new()
    {
        {Platforms.IOs, "🍎 iOS"},
        {Platforms.MacOs, "💻 macOS"},
        {Platforms.Android, "📱 Android"},
        {Platforms.Windows, "🖥 Windows/Linux"},
    };

    public override async Task Process(Update update)
    {
        var query = update.CallbackQuery!;
        var message = query.Message!;
        var chatId = message.Chat.Id;
        await Api.AnswerCallbackQueryAsync(new AnswerCallbackQueryArgs(update.CallbackQuery!.Id));
        var messageArgs = new EditMessageTextArgs("Выберите платформу:")
        {
            ChatId = chatId,
            MessageId = message.MessageId,
            ReplyMarkup = GetKeyboard()
        };
        await Api.EditMessageTextAsync<Message>(messageArgs);
    }

    private InlineKeyboardMarkup GetKeyboard()
    {
        var builder = BuildKeyboard();
        foreach (var keyValuePair in PlatformButtons)
        {
            var platform = keyValuePair.Key;
            var button = keyValuePair.Value;
            builder
                .AppendRow()
                .AppendCallbackData(button, $"{CallbackQueriesConstants.Instructions};get;{platform}");
        }

        builder
            .AppendRow()
            .AppendCallbackData("Главное меню", CallbackQueriesConstants.MainMenu);
        return builder.Build();
    }
}