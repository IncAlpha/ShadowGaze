using Microsoft.Extensions.Configuration;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace ShadowGaze.Core.Models;

public sealed class PublicBotProperties
{
    public TelegramBotClient Api { get; }
    public User User { get; }

    public PublicBotProperties(IConfiguration configuration)
    {
        var secret = configuration.GetSection("secret");
        var botToken = secret["token"];

        Api = new TelegramBotClient(new TelegramBotClientOptions(botToken!));

        User = Api.GetMe();
        Api.DeleteWebhook();
    }
}