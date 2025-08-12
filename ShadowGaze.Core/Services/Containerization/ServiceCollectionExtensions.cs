using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShadowGaze.Core.Models;
using ShadowGaze.Core.Services.UpdateProcessors;
using ShadowGaze.Core.Services.UpdateProcessors.Messages;
using ShadowGaze.Data.Services.Database;

namespace ShadowGaze.Core.Services.Containerization;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPublicBot(this IServiceCollection services, HostBuilderContext context)
    {
        return services
            .AddHostedService<BotBackgroundService>()
            .AddSingleton<IVersionProvider, VersionProvider>()
            .AddSingleton<PublicBotProperties>()
            .AddSingleton<SessionContextProvider>()
            .AddScoped<GeneralUpdateProcessor>()
            .AddNpgsql(context)
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

    private static IServiceCollection AddNpgsql(this IServiceCollection services, HostBuilderContext context)
    {
        return services.AddDbContext<DatabaseContext>(opt =>
        {
            opt.UseNpgsql(context.Configuration.GetConnectionString("Default"));
            opt.UseSnakeCaseNamingConvention();
        });
    }
}