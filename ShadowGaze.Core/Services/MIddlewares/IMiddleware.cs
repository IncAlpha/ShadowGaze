
using Telegram.BotAPI.GettingUpdates;

namespace ShadowGaze.Core.Services.MIddlewares;

public interface IMiddleware
{
    Task<bool> Process(Update update);
}