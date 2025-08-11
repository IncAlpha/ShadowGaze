using Microsoft.Extensions.Logging;
using ShadowGaze.Data.Models.TelegramApi;
using Telegram.BotAPI.Extensions;
using Telegram.BotAPI.GettingUpdates;

namespace ShadowGaze.Core.Services.UpdateProcessors;

public class GeneralUpdateProcessor
{
    private readonly ILogger _logger;
    private readonly SessionContextProvider _sessionContextProvider;
    private readonly IEnumerable<BaseUpdateProcessor> _updateProcessors;

    public GeneralUpdateProcessor(ILogger<GeneralUpdateProcessor> logger,
        SessionContextProvider sessionContextProvider,
        IEnumerable<BaseUpdateProcessor> updateProcessors)
    {
        _logger = logger;
        _sessionContextProvider = sessionContextProvider;
        _updateProcessors = updateProcessors;
    }

    public async Task Process(Update update, CancellationToken cancellationToken)
    {
        var message = update.Message;
        var sender = message?.From;
        var sessionContext = sender is not null ? _sessionContextProvider.GetContextForUser(sender.Id) : null;

        foreach (var processor in _updateProcessors)
        {
            var updateType = UpdateType.FromString(update.GetUpdateType());
            if (processor.Filter is not null &&
                !processor.Filter.Invoke(updateType, update, sessionContext))
            {
                continue;
            }

            await processor.Process(update);
        }
    }
}