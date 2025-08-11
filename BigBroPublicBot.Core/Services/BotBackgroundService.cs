using BigBroPublicBot.Core.Models;
using BigBroPublicBot.Core.Services.MessageProcessors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;

namespace BigBroPublicBot.Core.Services;

public class BotBackgroundService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IVersionProvider _versionProvider;
    private readonly BotClient _api;

    public BotBackgroundService(ILogger<BotBackgroundService> logger,
        IBotProperties botProperties,
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

        var updates = await _api.GetUpdatesAsync(cancellationToken: stoppingToken).ConfigureAwait(false);
        while (!stoppingToken.IsCancellationRequested)
        {
            if (updates.Any())
            {
                await Parallel.ForEachAsync(updates, stoppingToken,
                    async (update, token) => await ProcessUpdate(update, token)
                );

                updates = await _api.GetUpdatesAsync(updates[^1].UpdateId + 1, cancellationToken: stoppingToken)
                    .ConfigureAwait(false);
            }
            else
            {
                updates = await _api.GetUpdatesAsync(cancellationToken: stoppingToken).ConfigureAwait(false);
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