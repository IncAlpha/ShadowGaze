using Microsoft.Extensions.Logging;
using Telegram.BotAPI.GettingUpdates;

namespace ShadowGaze.Core.Services.MessageProcessors;

public class GeneralUpdateProcessor
{
    private readonly ILogger _logger;
    private readonly SessionContextProvider _sessionContextProvider;
    private readonly IEnumerable<BaseMessageProcessor> _messageProcessors;

    public GeneralUpdateProcessor(ILogger<GeneralUpdateProcessor> logger,
        SessionContextProvider sessionContextProvider,
        IEnumerable<BaseMessageProcessor> messageProcessors)
    {
        _logger = logger;
        _sessionContextProvider = sessionContextProvider;
        _messageProcessors = messageProcessors;
    }

    public async Task Process(Update update, CancellationToken cancellationToken)
    {
        var message = update.Message;
        var sender = message?.From;
        var sessionContext = sender is not null ? _sessionContextProvider.GetContextForUser(sender.Id) : null;

        foreach (var processor in _messageProcessors)
        {
            if (processor.UpdateTypeFilter is not null && 
                !processor.UpdateTypeFilter.Invoke(update.Type))
            {
                continue;
            }

            if (processor.MessageFilter is not null )
            {
                
            }

            await processor.Process(update.Message);
        }
    }
}