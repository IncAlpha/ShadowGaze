using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;

namespace ShadowGaze.Core.Services.Extensions;

public static class KeyboardBuilderExtensions
{
    public static InlineKeyboardBuilder BuildKeyboard()
    {
        return new InlineKeyboardBuilder();
    }

    public static InlineKeyboardMarkup Build(this InlineKeyboardBuilder builder)
    {
        return new InlineKeyboardMarkup(builder);
    }
}