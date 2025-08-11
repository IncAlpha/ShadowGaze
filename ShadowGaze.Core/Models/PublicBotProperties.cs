using Microsoft.Extensions.Configuration;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace ShadowGaze.Core.Models;

public sealed class PublicBotProperties : IBotProperties
{
    IBotCommandHelper IBotProperties.CommandHelper => _botCommandHelper;

    public BotClient Api { get; }
    public User User { get; }

    private readonly BotCommandHelper _botCommandHelper;

    public PublicBotProperties(IConfiguration configuration)
    {
        var secret = configuration.GetSection("secret");
        var botToken = secret["token"];

        Api = new BotClient(botToken);

        User = Api.GetMe();
        Api.DeleteWebhook();

        _botCommandHelper = new BotCommandHelper(this);
    }
}