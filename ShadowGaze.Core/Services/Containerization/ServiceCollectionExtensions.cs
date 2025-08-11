using Microsoft.Extensions.DependencyInjection;
using ShadowGaze.Core.Models;
using ShadowGaze.Core.Services.UpdateProcessors;
using ShadowGaze.Core.Services.UpdateProcessors.Messages;
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
            .AddScoped<GeneralUpdateProcessor>()
            .AddHttp()
            .AddUpdateProcessors();
    }

    private static IServiceCollection AddUpdateProcessors(this IServiceCollection services)
    {
        return services
            .AddScoped<BaseUpdateProcessor, BaseMessageProcessor>();
    }

    private static IServiceCollection AddHttp(this IServiceCollection services)
    {
        return services
            .AddScoped<HttpClient>();
    }
}