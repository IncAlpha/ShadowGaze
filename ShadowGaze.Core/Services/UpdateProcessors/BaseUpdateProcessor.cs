using ShadowGaze.Core.Models;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors;

public abstract class BaseUpdateProcessor(PublicBotProperties botProperties)
{
    public abstract Func<UpdateTypes, Update, SessionContext, bool> Filter { get; }

    public abstract Task Process(Update update);

    protected ITelegramBotClient Api => botProperties.Api;
}