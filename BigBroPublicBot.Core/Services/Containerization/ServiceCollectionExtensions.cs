using BigBroPublicBot.Core.Models;
using BigBroPublicBot.Core.Services.MessageProcessors;
using Microsoft.Extensions.DependencyInjection;
using Telegram.BotAPI;

namespace BigBroPublicBot.Core.Services.Containerization;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPublicBot(this IServiceCollection services)
    {
        return services
            .AddHostedService<BotBackgroundService>()
            .AddSingleton<IVersionProvider, VersionProvider>()
            .AddSingleton<IBotProperties, PublicBotProperties>()
            .AddSingleton<SessionContextProvider>()
            .AddSingleton(provider => provider.GetRequiredService<IBotProperties>().Api)
            .AddSingleton<BaseMessageProcessor, AnnouncementProcessor>()
            .AddScoped<GeneralUpdateProcessor>();
    }
}