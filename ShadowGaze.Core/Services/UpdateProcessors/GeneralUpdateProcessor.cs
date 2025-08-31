using Microsoft.Extensions.Logging;
using ShadowGaze.Core.Services.Middlewares;
using ShadowGaze.Data.Models.TelegramApi;
using Telegram.BotAPI.Extensions;
using Telegram.BotAPI.GettingUpdates;

namespace ShadowGaze.Core.Services.UpdateProcessors;

public class GeneralUpdateProcessor(
    ILogger<GeneralUpdateProcessor> logger,
    SessionContextProvider sessionContextProvider,
    IEnumerable<BaseUpdateProcessor> updateProcessors,
    IEnumerable<IMiddleware> middlewares
)
{
    private readonly ILogger _logger = logger;

    public async Task Process(Update update, CancellationToken cancellationToken)
    {
        var sender = update.Message?.From ?? update.CallbackQuery?.From;
        var sessionContext = sender is not null ? sessionContextProvider.GetContextForUser(sender.Id) : null;

        foreach (var middleware in middlewares)
        {
            if (!await middleware.Process(update))
            {
                return;
            }
        }

        foreach (var processor in updateProcessors)
        {
            try
            {
                var updateType = UpdateType.FromString(update.GetUpdateType());
                if (processor.Filter is not null &&
                    !processor.Filter.Invoke(updateType, update, sessionContext))
                {
                    continue;
                }

                await processor.Process(update, sessionContext);
                break;
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, $"Возникла ошибка при работе {processor.GetType().Name}");
            }
        }
    }
}