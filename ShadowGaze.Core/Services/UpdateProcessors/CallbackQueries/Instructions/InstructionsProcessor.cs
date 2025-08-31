using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Core.Models.SessionContexts;
using ShadowGaze.Core.Services.Extensions;
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
        { Platforms.IOs, "🍎 iOS" },
        { Platforms.MacOs, "💻 macOS" },
        { Platforms.Android, "📱 Android" },
        { Platforms.Windows, "🖥 Windows/Linux" },
    };

    public override async Task Process(Update update, SessionContext sessionContext)
    {
        var query = update.CallbackQuery!;
        var message = (query.Message as Message)!;
        var chatId = message.Chat.Id;
        await Bot.AnswerCallbackQueryAsync(new AnswerCallbackQueryArgs(update.CallbackQuery!.Id));
        var messageArgs = new EditMessageTextArgs("Выберите платформу:")
        {
            ChatId = chatId,
            MessageId = message.MessageId,
            ReplyMarkup = GetKeyboard()
        };

        if (message.HasMedia())
        {
            await Bot.RemoveLastButton(message);
            await Bot.SendMessageAsync(messageArgs);
            return;
        }

        await Bot.EditMessageTextAsync<Message>(messageArgs);
    }

    private InlineKeyboardMarkup GetKeyboard()
    {
        var builder = BuildKeyboard();
        foreach (var (platform, button) in PlatformButtons)
        {
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