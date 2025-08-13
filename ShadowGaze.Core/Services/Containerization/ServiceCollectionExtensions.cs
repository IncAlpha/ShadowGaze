using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShadowGaze.Core.Models;
using ShadowGaze.Core.Services.UpdateProcessors;
using ShadowGaze.Core.Services.UpdateProcessors.CallbackQuery;
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
            .AddDatabase(context)
            .AddRepositories()
            .AddHttp()
            .AddUpdateProcessors();
    }

    private static IServiceCollection AddUpdateProcessors(this IServiceCollection services)
    {
        return services
            .AddScoped<BaseUpdateProcessor, CommandStartMessageProcessor>()
            .AddScoped<BaseUpdateProcessor, GetLinkCallbackQueryProcessor>();
    }

    private static IServiceCollection AddHttp(this IServiceCollection services)
    {
        return services
            .AddScoped<HttpClient>();
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, HostBuilderContext context)
    {
        return services.AddDbContext<DatabaseContext>(opt =>
        {
            opt.UseNpgsql(context.Configuration.GetConnectionString("Default"));
            opt.UseSnakeCaseNamingConvention();
        });
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<CustomersRepository>();
        services.AddScoped<EndpointsRepository>();
        services.AddScoped<XrayRepository>();
        return services;
    }
}