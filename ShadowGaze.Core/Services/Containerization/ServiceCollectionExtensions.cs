using Microsoft.Extensions.DependencyInjection;
using ShadowGaze.Core.Models;
using ShadowGaze.Core.Services.MessageProcessors;
using Telegram.BotAPI;

namespace ShadowGaze.Core.Services.Containerization;

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