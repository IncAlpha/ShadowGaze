using ShadowGaze.Core.Models;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;

namespace ShadowGaze.Core.Services.MessageProcessors;

public abstract class BaseMessageProcessor
{
    public Predicate<UpdateType> UpdateTypeFilter { get; protected set; } = null;
    public Predicate<Message> MessageFilter { get; protected set; } = null;
    public Predicate<SessionContext> SessionContextFilter { get; protected set; } = null;

    public abstract Task Process(Message message);
}