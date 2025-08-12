using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ShadowGaze.Core.Models;
using ShadowGaze.Core.Services.UpdateProcessors;
using ShadowGaze.Core.Services.UpdateProcessors.Messages;
using ShadowGaze.Core.Services.XUI;
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
            .AddSingleton<AuthService>()
            .AddDatabase(context)
            .AddRepositories()
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
            .AddSingleton<CookieContainer>()
            .AddHttpClient(Options.DefaultName)
            .ConfigurePrimaryHttpMessageHandler((sp) =>
            {
                var cookieContainer = sp.GetRequiredService<CookieContainer>();
                return new HttpClientHandler
                {
                    UseCookies = true,
                    CookieContainer = cookieContainer,
                    UseProxy = true,
                    Proxy = new WebProxy("http://localhost:2080")

                };
            }).Services;
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