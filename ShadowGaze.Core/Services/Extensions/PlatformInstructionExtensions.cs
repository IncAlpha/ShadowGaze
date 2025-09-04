using ShadowGaze.Data.Models.Database.Instructions;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;
using Telegram.BotAPI.UpdatingMessages;

namespace ShadowGaze.Core.Services.Extensions;

public static class PlatformInstructionExtensions
{
    public static SendVideoArgs BuildSendMessageArgs(this PlatformInstruction platformInstruction, long chatId,
        Action<InlineKeyboardBuilder> postfixButton = null)
    {
        var builder = BuildKeyboard()
            .AppendUrl(platformInstruction.ApplicationName, platformInstruction.ApplicationUrl);
        postfixButton?.Invoke(builder);
        return new SendVideoArgs(chatId, platformInstruction.TelegramFile.FileId)
        {
            ParseMode = "MarkdownV2",
            Caption = platformInstruction.Description,
            ReplyMarkup = builder.Build()
        };
    }

    public static EditMessageMediaArgs BuildEditMessageArgs(this PlatformInstruction platformInstruction, long chatId,
        int messageId, Action<InlineKeyboardBuilder> postfixButton = null)
    {
        var media = new InputMediaVideo(platformInstruction.TelegramFile.FileId)
        {
            Caption = platformInstruction.Description,
            ParseMode = "MarkdownV2",
        };
        var builder = BuildKeyboard()
            .AppendUrl(platformInstruction.ApplicationName, platformInstruction.ApplicationUrl);
        postfixButton?.Invoke(builder);
        return new EditMessageMediaArgs(media)
        {
            ChatId = chatId,
            MessageId = messageId,
            ReplyMarkup = builder.Build()
        };
    }
}