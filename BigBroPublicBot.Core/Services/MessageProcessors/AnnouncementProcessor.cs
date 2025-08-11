using Microsoft.Extensions.Logging;
using Telegram.BotAPI.AvailableTypes;

namespace BigBroPublicBot.Core.Services.MessageProcessors;

public class AnnouncementProcessor : BaseMessageProcessor
{
    private readonly ILogger _logger;

    public AnnouncementProcessor(ILogger<AnnouncementProcessor> logger)
    {
        _logger = logger;
        MessageFilter = message => message.Text.StartsWith("!");
    }
    
    public override Task Process(Message message)
    {
        _logger.LogInformation("Process");
        return Task.CompletedTask;
    }
}