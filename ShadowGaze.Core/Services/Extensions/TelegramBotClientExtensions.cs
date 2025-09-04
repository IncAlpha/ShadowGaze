using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.UpdatingMessages;

namespace ShadowGaze.Core.Services.Extensions;

public static class TelegramBotClientExtensions
{
    public static async Task<Message> SendMessageAsync(this ITelegramBotClient bot, EditMessageTextArgs messageArgs)
    {
        if (messageArgs.ChatId is not long chatId)
        {
            return null;
        }

        var sendMessageArgs = new SendMessageArgs(chatId, messageArgs.Text)
        {
            ReplyMarkup = messageArgs.ReplyMarkup,
            ParseMode = messageArgs.ParseMode,
        };
        return await bot.SendMessageAsync(sendMessageArgs);
    }
    
    public static async Task<Message> EditMessageAsync(this ITelegramBotClient bot, int messageId, SendMessageArgs messageArgs)
    {
        if (messageArgs.ChatId is not long chatId)
        {
            return null;
        }

        var editMessageArgs = new EditMessageTextArgs(messageArgs.Text)
        {
            ChatId = messageArgs.ChatId,
            MessageId = messageId,
            ReplyMarkup = (InlineKeyboardMarkup) messageArgs.ReplyMarkup,
            ParseMode = messageArgs.ParseMode,
        };
        return await bot.EditMessageTextAsync<Message>(editMessageArgs);
    }

    public static async Task<Message> RemoveLastButton(this ITelegramBotClient bot, Message message)
    {
        if (message.ReplyMarkup is null)
        {
            return message;
        }

        var messageKeyboard = message.ReplyMarkup.InlineKeyboard;
        var newKeyboard = message.ReplyMarkup.InlineKeyboard.Take(messageKeyboard.Count() - 1);
        var editArgs = new EditMessageReplyMarkupArgs
        {
            ChatId = message.Chat.Id,
            MessageId = message.MessageId,
            ReplyMarkup = new InlineKeyboardMarkup(newKeyboard)
        };
        return await bot.EditMessageReplyMarkupAsync<Message>(editArgs);
    }

    public static async Task<Message> SendFileAsync(this ITelegramBotClient bot, AttachedFilesArgsBase fileArgs)
    {
        return fileArgs switch
        {
            SendAnimationArgs args => await bot.SendAnimationAsync(args),
            SendVideoArgs args => await bot.SendVideoAsync(args),
            SendDocumentArgs args => await bot.SendDocumentAsync(args),
            SendAudioArgs args => await bot.SendAudioAsync(args),
            SendPhotoArgs args => await bot.SendPhotoAsync(args),
            _ => throw new ArgumentException(null, nameof(fileArgs))
        };
    }
}