using BigBroPublicBot.Core.Models;

namespace BigBroPublicBot.Core.Services.Containerization;

public static class HostBuilderExtensions
{
    public static IHostBuilder AddPublicBot(this IHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureHostConfiguration(builder => { builder.AddJsonFile("secret.json"); })
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<PublicBotProperties>();
                services.AddScoped<PublicBot>();
                services.AddHostedService<BotBackgroundService>();
            });
    }
}