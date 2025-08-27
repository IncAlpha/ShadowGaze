using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Congifurations;
using ShadowGaze.Core.Services.Middlewares;
using ShadowGaze.Core.Services.UpdateProcessors;
using ShadowGaze.Core.Services.UpdateProcessors.CallbackQueries.Accounts;
using ShadowGaze.Core.Services.UpdateProcessors.CallbackQueries.Endpoints;
using ShadowGaze.Core.Services.UpdateProcessors.CallbackQueries.Instructions;
using ShadowGaze.Core.Services.UpdateProcessors.CallbackQueries.Subscriptions;
using ShadowGaze.Core.Services.UpdateProcessors.Messages;
using ShadowGaze.Core.Services.XUI;
using ShadowGaze.Data.Services.Database;
using ShadowGaze.Data.Services.Database.Instructions;

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
            .AddSingleton<XuiService>()
            .AddDatabase(context)
            .AddBotOptions(context)
            .AddRepositories()
            .AddHttp()
            .AddUpdateProcessors()
            .AddMiddleware();
    }

    private static IServiceCollection AddUpdateProcessors(this IServiceCollection services)
    {
        return services
            .AddScoped<BaseUpdateProcessor, MainMenuCallbackProcessor>()
            .AddScoped<BaseUpdateProcessor, MainMenuMessageProcessor>()
            .AddScoped<BaseUpdateProcessor, EndpointsProcessor>()
            .AddScoped<BaseUpdateProcessor, SubscriptionsCallbackQueryProcessor>()
            .AddScoped<BaseUpdateProcessor, SelectSubscriptionsCallbackQueryProcessor>()
            .AddScoped<BaseUpdateProcessor, AccountCallbackQueryProcessor>()
            .AddScoped<BaseUpdateProcessor, AccountTopUpCallbackQueryProcessor>()
            .AddScoped<BaseUpdateProcessor, InstructionsProcessor>()
            .AddScoped<BaseUpdateProcessor, GetInstructionsProcessor>();
    }

    private static IServiceCollection AddMiddleware(this IServiceCollection services)
    {
        return services
            .AddScoped<IMiddleware, AdminCommandsMiddleware>();
    }

    private static IServiceCollection AddHttp(this IServiceCollection services)
    {
        return services
            .AddSingleton<CookieContainer>()
            .AddHttpClient(Options.DefaultName)
            .ConfigurePrimaryHttpMessageHandler(provider =>
            {
                var cookieContainer = provider.GetRequiredService<CookieContainer>();
                return new HttpClientHandler
                {
                    UseCookies = true,
                    CookieContainer = cookieContainer,
                    // UseProxy = true,
                    // Proxy = new WebProxy("http://localhost:2081")
                };
            })
            .RemoveAllLoggers()
            .Services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, HostBuilderContext context)
    {
        return services.AddDbContextFactory<DatabaseContext>(opt =>
        {
            opt.UseNpgsql(context.Configuration.GetConnectionString("Default"));
            opt.UseSnakeCaseNamingConvention();
        });
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services.AddScoped<CustomersRepository>()
            .AddScoped<EndpointsRepository>()
            .AddScoped<XrayRepository>()
            .AddScoped<PlatformInstructionsRepository>();
    }

    private static IServiceCollection AddBotOptions(this IServiceCollection services, HostBuilderContext context)
    {
        return services.Configure<XUiOptions>(context.Configuration.GetSection(nameof(XUiOptions)));
    }
}