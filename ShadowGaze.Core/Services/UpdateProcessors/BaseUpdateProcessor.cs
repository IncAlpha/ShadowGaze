using ShadowGaze.Core.Models;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors;

public abstract class BaseUpdateProcessor
{
    public abstract Func<UpdateTypes, Update, SessionContext, bool> Filter { get; }

    public abstract Task Process(Update update);
}