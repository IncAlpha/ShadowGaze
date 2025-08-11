using ShadowGaze.Core.Models;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors;

public abstract class BaseUpdateProcessor
{
    public abstract Func<UpdateType, Message, SessionContext, bool> Filter { get; }

    public abstract Task Process(Message message);
}