using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;

namespace ShadowGaze.Core.Services.Extensions;

public static class MessageExtensions
{
    /// <summary>
    /// Возвращает текст сообщения, отформатированный в Markdown с экранированием для Telegram.
    /// </summary>
    public static string GetMarkdownText(this Message message)
    {
        if (string.IsNullOrEmpty(message.Text) || 
            message.Entities == null || 
            !message.Entities.Any())
        {
            return (message.Text ?? string.Empty).EscapeMarkdownSpecial();
        }

        var text = message.Text;
        var entities = message.Entities.OrderBy(e => e.Offset).ToList();
        var result = new System.Text.StringBuilder();
        var currentIndex = 0;

        foreach (var entity in entities)
        {
            // Добавляем текст до entity
            if (entity.Offset > currentIndex)
            {
                result.Append(text.AsSpan(currentIndex, entity.Offset - currentIndex).ToString().EscapeMarkdownSpecial());
            }

            var entityText = text.Substring(entity.Offset, entity.Length);

            switch (entity.Type)
            {
                case MessageEntityTypes.Bold:
                    result.Append($"*{entityText.EscapeMarkdownSpecial()}*");
                    break;
                case MessageEntityTypes.Italic:
                    result.Append($"_{entityText.EscapeMarkdownSpecial()}_");
                    break;
                case MessageEntityTypes.Underline:
                    result.Append($"__{entityText.EscapeMarkdownSpecial()}__");
                    break;
                case MessageEntityTypes.Strikethrough:
                    result.Append($"~{entityText.EscapeMarkdownSpecial()}~");
                    break;
                case MessageEntityTypes.Code:
                    result.Append($"`{entityText.EscapeMarkdownSpecial()}`");
                    break;
                case MessageEntityTypes.Pre:
                    result.Append($"```\n{entityText.EscapeMarkdownSpecial()}\n```");
                    break;
                case MessageEntityTypes.TextLink:
                    result.Append($"[{entityText.EscapeMarkdownSpecial()}]({entity.Url.EscapeMarkdownSpecial()})");
                    break;
                default:
                    result.Append(entityText.EscapeMarkdownSpecial());
                    break;
            }

            currentIndex = entity.Offset + entity.Length;
        }

        // Добавляем оставшийся текст
        if (currentIndex < text.Length)
        {
            result.Append(text.AsSpan(currentIndex).ToString().EscapeMarkdownSpecial());
        }

        return result.ToString();
    }

    public static string GetMarkdownTextClear(this Message message)
    {
        if (string.IsNullOrEmpty(message.Text) || 
            message.Entities == null || 
            !message.Entities.Any())
        {
            return message.Text ?? string.Empty;
        }

        var text = message.Text;
        var entities = message.Entities.OrderBy(e => e.Offset).ToList();
        var result = new System.Text.StringBuilder();
        var currentIndex = 0;

        foreach (var entity in entities)
        {
            // Добавляем текст до entity
            if (entity.Offset > currentIndex)
            {
                result.Append(text.AsSpan(currentIndex, entity.Offset - currentIndex));
            }

            var entityText = text.Substring(entity.Offset, entity.Length);

            switch (entity.Type)
            {
                case MessageEntityTypes.Bold:
                    result.Append($"**{entityText}**");
                    break;
                case MessageEntityTypes.Italic:
                    result.Append($"*{entityText}*");
                    break;
                case MessageEntityTypes.Underline:
                    result.Append($"__{entityText}__");
                    break;
                case MessageEntityTypes.Strikethrough:
                    result.Append($"~~{entityText}~~");
                    break;
                case MessageEntityTypes.Code:
                    result.Append($"`{entityText}`");
                    break;
                case MessageEntityTypes.Pre:
                    result.Append($"```\n{entityText}\n```");
                    break;
                case MessageEntityTypes.TextLink:
                    result.Append($"[{entityText}]({entity.Url})");
                    break;
                default:
                    result.Append(entityText);
                    break;
            }

            currentIndex = entity.Offset + entity.Length;
        }

        // Добавляем оставшийся текст
        if (currentIndex < text.Length)
        {
            result.Append(text.AsSpan(currentIndex));
        }

        return result.ToString();
    }

    public static bool HasMedia(this Message target)
    {
        return target.Animation is not null || 
               target.Audio is not null || 
               target.Document is not null || 
               target.Photo is not null  || 
               target.Video is not null || 
               target.VideoNote is not null || 
               target.Voice is not null ||
               target.MediaGroupId is not null;
    }
}