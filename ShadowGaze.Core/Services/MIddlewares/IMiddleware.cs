
using Telegram.BotAPI.GettingUpdates;

namespace ShadowGaze.Core.Services.Middlewares;

public interface IMiddleware
{
    Task<bool> Process(Update update);
}