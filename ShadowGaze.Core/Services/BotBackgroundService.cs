using ShadowGaze.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ShadowGaze.Core.Services.UpdateProcessors;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;

namespace ShadowGaze.Core.Services;

public class BotBackgroundService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IVersionProvider _versionProvider;
    private readonly ITelegramBotClient _api;

    public BotBackgroundService(ILogger<BotBackgroundService> logger,
        PublicBotProperties botProperties,
        IServiceProvider serviceProvider,
        IVersionProvider versionProvider)
    {
        _logger = logger;
        _api = botProperties.Api;
        _serviceProvider = serviceProvider;
        _versionProvider = versionProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"Запуск бота [{_versionProvider.FileVersionInfo.FileVersion}]");
        var updates = (await _api
                .GetUpdatesAsync(cancellationToken: stoppingToken)
                .ConfigureAwait(false))
            .ToArray();
        while (!stoppingToken.IsCancellationRequested)
        {
            if (updates.Length != 0)
            {
                try // todo 
                    // If an error occurs while processing the callback,
                    // the telegram api can send the callback again and throw an exception when trying to process it
                    // Use webhook
                {
                    await Parallel.ForEachAsync(updates, stoppingToken,
                        async (update, token) => await ProcessUpdate(update, token)
                    );
                }
                catch (BotRequestException e)
                {
                    _logger.LogError(e.Message);
                }
                

                updates = (await _api.GetUpdatesAsync(updates[^1].UpdateId + 1, cancellationToken: stoppingToken)
                        .ConfigureAwait(false))
                    .ToArray();
            }
            else
            {
                updates = (await _api.GetUpdatesAsync(cancellationToken: stoppingToken).ConfigureAwait(false))
                    .ToArray();
            }
        }
    }

    private async Task ProcessUpdate(Update update, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var updateProcessor = scope.ServiceProvider.GetRequiredService<GeneralUpdateProcessor>();
        await updateProcessor.Process(update, cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Worker stopping at: {Time}", DateTimeOffset.Now);
        return base.StopAsync(cancellationToken);
    }
}