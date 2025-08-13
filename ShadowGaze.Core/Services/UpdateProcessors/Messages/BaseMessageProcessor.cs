using ShadowGaze.Core.Models;
using Telegram.BotAPI;

namespace ShadowGaze.Core.Services.UpdateProcessors.Messages;

public abstract class BaseMessageProcessor(PublicBotProperties botProperties) : BaseUpdateProcessor
{
    protected readonly ITelegramBotClient Api = botProperties.Api;
}