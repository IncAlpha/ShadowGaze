using BigBroPublicBot.Core.Models;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;

namespace BigBroPublicBot.Core.Services;

public class PublicBot : TelegramBotBase<PublicBotProperties>
{
    private readonly ILogger _logger;

    public PublicBot(ILogger<PublicBot> logger, PublicBotProperties botProperties) : base(botProperties)
    {
        _logger = logger;
    }

    protected override void OnBotException(BotRequestException exception)
    {
    }

    protected override void OnException(Exception exception)
    {
    }

    protected override void OnCommand(Message message, string commandName, string commandParameters)
    {
    }

    protected override void OnMessage(Message message)
    {
        // Ignore user 777000 (Telegram)
        if (message!.From?.Id == TelegramConstants.TelegramId)
        {
            return;
        }

        var hasText = !string.IsNullOrEmpty(message.Text); // True if message has text;

#if DEBUG
        _logger.LogInformation("New message from chat id: {ChatId}", message!.Chat.Id);
        _logger.LogInformation("Message: {MessageContent}", hasText ? message.Text : "No text");
#endif

        base.OnMessage(message);
    }
}