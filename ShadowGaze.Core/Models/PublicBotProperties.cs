using Microsoft.Extensions.Configuration;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;

namespace ShadowGaze.Core.Models;

public sealed class PublicBotProperties
{
    public TelegramBotClient Bot { get; }

    public PublicBotProperties(IConfiguration configuration)
    {
        var secret = configuration.GetSection("secret");
        var botToken = secret["token"];

        Bot = new TelegramBotClient(new TelegramBotClientOptions(botToken!));

        Bot.DeleteWebhook();
    }
}