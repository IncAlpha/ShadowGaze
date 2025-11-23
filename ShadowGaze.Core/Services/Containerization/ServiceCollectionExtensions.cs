using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Congifurations;
using ShadowGaze.Core.Services.Middlewares;
using ShadowGaze.Core.Services.Subscriptions;
using ShadowGaze.Core.Services.UpdateProcessors;
using ShadowGaze.Core.Services.UpdateProcessors.CallbackQueries.Connections;
using ShadowGaze.Core.Services.UpdateProcessors.CallbackQueries.Instructions;
using ShadowGaze.Core.Services.UpdateProcessors.CallbackQueries.PromotionalCodes;
using ShadowGaze.Core.Services.UpdateProcessors.CallbackQueries.Subscriptions;
using ShadowGaze.Core.Services.UpdateProcessors.Messages.AdminCommands.AddPlatformInstruction;
using ShadowGaze.Core.Services.UpdateProcessors.Messages.AdminCommands.GetFileId;
using ShadowGaze.Core.Services.UpdateProcessors.Messages.AdminCommands.XrayCore;
using ShadowGaze.Core.Services.UpdateProcessors.Messages.MainMenu;
using ShadowGaze.Core.Services.UpdateProcessors.Payments;
using ShadowGaze.Core.Services.Xray;
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
            .AddSingleton<SubscriptionMatches>()
            .AddDatabase(context)
            .AddBotOptions(context)
            .AddRepositories()
            .AddHttp()
            .AddGrpc()
            .AddNotification()
            .AddMiddleware()
            .AddUpdateProcessors();
    }

    private static IServiceCollection AddUpdateProcessors(this IServiceCollection services)
    {
        return services
            .AddScoped<BaseUpdateProcessor, MainMenuCallbackProcessor>()
            .AddScoped<BaseUpdateProcessor, MainMenuMessageProcessor>()
            .AddScoped<BaseUpdateProcessor, MainConnectionProcessor>()
            .AddScoped<BaseUpdateProcessor, ConnectionsListProcessor>()
            .AddScoped<BaseUpdateProcessor, ConnectionForInboundProcessor>()
            .AddScoped<BaseUpdateProcessor, SubscriptionsCallbackQueryProcessor>()
            .AddScoped<BaseUpdateProcessor, SelectSubscriptionsCallbackQueryProcessor>()

            // платежи
            .AddScoped<BaseUpdateProcessor, PreCheckoutQueryProcessor>()
            .AddScoped<BaseUpdateProcessor, CheckoutMessageProcessor>()

            // инструкции
            .AddScoped<BaseUpdateProcessor, InstructionsProcessor>()
            .AddScoped<BaseUpdateProcessor, GetInstructionsProcessor>()
            .AddScoped<BaseUpdateProcessor, GetInstructionByIdProcessor>()
            .AddScoped<BaseUpdateProcessor, AddPlatformInstructionProcessor>()
            .AddScoped<BaseUpdateProcessor, SetPlatformProcessor>()
            .AddScoped<BaseUpdateProcessor, SetDescriptionProcessor>()
            .AddScoped<BaseUpdateProcessor, SetFileProcessor>()

            //промокоды
            .AddScoped<BaseUpdateProcessor, PromotionalCodeCallbackQueryProcessor>()
            .AddScoped<BaseUpdateProcessor, PromotionalCodeMessageUpdateProcessor>()

            // fileId
            .AddScoped<BaseUpdateProcessor, FileProcessor>()
            .AddScoped<BaseUpdateProcessor, MediaProcessor>()

            // управление
            .AddScoped<BaseUpdateProcessor, SyncXrayProcessor>();
    }

    private static IServiceCollection AddMiddleware(this IServiceCollection services)
    {
        return services
            .AddScoped<IMiddleware, AdminCommandsMiddleware>();
    }

    private static IServiceCollection AddGrpc(this IServiceCollection services)
    {
        return services
            .AddScoped<XrayService>()
            .AddSingleton<IXrayClientFactory, XrayClientFactory>()
            .AddHostedService<XraySyncService>();
    }

    private static IServiceCollection AddNotification(this IServiceCollection services)
    {
        return services
            .AddScoped<NotificationService>()
            .AddHostedService<UserNotificationService>();
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
                    // Proxy = new WebProxy("socks5://localhost:2080")
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
            .AddScoped<PlatformInstructionsRepository>()
            .AddScoped<BotSectionsRepository>()
            .AddScoped<TelegramFilesRepository>()
            .AddScoped<PaymentsRepository>()
            .AddScoped<PromotionalCodeRepository>()
            .AddScoped<PromotionalCodeUsageRepository>()
            .AddScoped<ConnectionsRepository>()
            .AddScoped<InboundConfigurationRepository>()
            .AddScoped<InboundButtonRepository>()
            .AddScoped<XrayApiRepository>();
    }

    private static IServiceCollection AddBotOptions(this IServiceCollection services, HostBuilderContext context)
    {
        return services
            .Configure<XUiOptions>(context.Configuration.GetSection(nameof(XUiOptions)))
            .Configure<ConnectionsOptions>(context.Configuration.GetSection(nameof(ConnectionsOptions)));
    }
}