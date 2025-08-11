namespace ShadowGaze.Data.Models.TelegramApi;

public enum UpdateTypes
{
    Message,
    EditedMessage,
    ChannelPost,
    EditedChannelPost,
    BusinessConnection,
    BusinessMessage,
    EditedBusinessMessage,
    DeletedBusinessMessages,
    MessageReaction,
    MessageReactionCount,
    InlineQuery,
    ChosenInlineResult,
    CallbackQuery,
    ShippingQuery,
    PreCheckoutQuery,
    Poll,
    PollAnswer,
    MyChatMember,
    ChatMember,
    ChatJoinRequest,
    ChatBoost,
    RemovedChatBoost
}