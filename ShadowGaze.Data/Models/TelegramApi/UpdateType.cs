namespace ShadowGaze.Data.Models.TelegramApi;

public static class UpdateType
{
    public static UpdateTypes FromString(string value) => value switch
    {
        "message" => UpdateTypes.Message,
        "edited_message" => UpdateTypes.EditedMessage,
        "channel_post" => UpdateTypes.ChannelPost,
        "edited_channel_post" => UpdateTypes.EditedChannelPost,
        "business_connection" => UpdateTypes.BusinessConnection,
        "business_message" => UpdateTypes.BusinessMessage,
        "edited_business_message" => UpdateTypes.EditedBusinessMessage,
        "deleted_business_messages" => UpdateTypes.DeletedBusinessMessages,
        "message_reaction" => UpdateTypes.MessageReaction,
        "message_reaction_count" => UpdateTypes.MessageReactionCount,
        "inline_query" => UpdateTypes.InlineQuery,
        "chosen_inline_result" => UpdateTypes.ChosenInlineResult,
        "callback_query" => UpdateTypes.CallbackQuery,
        "shipping_query" => UpdateTypes.ShippingQuery,
        "pre_checkout_query" => UpdateTypes.PreCheckoutQuery,
        "poll" => UpdateTypes.Poll,
        "poll_answer" => UpdateTypes.PollAnswer,
        "my_chat_member" => UpdateTypes.MyChatMember,
        "chat_member" => UpdateTypes.ChatMember,
        "chat_join_request" => UpdateTypes.ChatJoinRequest,
        "chat_boost" => UpdateTypes.ChatBoost,
        "removed_chat_boost" => UpdateTypes.RemovedChatBoost,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
    };
}