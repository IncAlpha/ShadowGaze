using ShadowGaze.Core.Models;
using Telegram.BotAPI;

namespace ShadowGaze.Core.Services.UpdateProcessors.CallbackQuery;

public abstract class BaseCallbackQueryProcessor(PublicBotProperties botProperties) : BaseUpdateProcessor
{
    protected readonly ITelegramBotClient Api = botProperties.Api;
}