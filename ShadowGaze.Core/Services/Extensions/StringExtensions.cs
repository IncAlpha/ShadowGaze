namespace ShadowGaze.Core.Services.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Экранирует все специальные символы для MarkdownV2 Telegram.
    /// </summary>
    public static string EscapeMarkdownSpecial(this string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        char[] charsToEscape =
        [
            '_', '*', '[', ']', '(', ')',
            '~', '`', '>', '#', '+', '-',
            '=', '|', '{', '}', '.', '!'
        ];
        var result = new System.Text.StringBuilder(text.Length * 2);

        foreach (var @char in text)
        {
            if (charsToEscape.Contains(@char))
                result.Append('\\');
            result.Append(@char);
        }

        return result.ToString();
    }
    
    /// <summary>
    /// Экранирует знаки препинания и обычные символы для сообщений MarkdownV2 Telegram.
    /// </summary>
    public static string EscapeMarkdownCommon(this string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        char[] charsToEscape =
        [
            '>', '#', '+', '-',
            '=', '|', '.', '!', '>',
            '(', ')', '[', ']', '{', '}',
        ];
        var result = new System.Text.StringBuilder(text.Length * 2);

        foreach (var @char in text)
        {
            if (charsToEscape.Contains(@char))
                result.Append('\\');
            result.Append(@char);
        }

        return result.ToString();
    }
}