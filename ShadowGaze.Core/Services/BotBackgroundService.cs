using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using ShadowGaze.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ShadowGaze.Core.Services.UpdateProcessors;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;

namespace ShadowGaze.Core.Services;

public class BotBackgroundService : BackgroundService
{
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(5);
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IVersionProvider _versionProvider;
    private readonly IDbContextFactory<DatabaseContext> _dbFactory;
    private readonly ITelegramBotClient _bot;

    public BotBackgroundService(ILogger<BotBackgroundService> logger,
        PublicBotProperties botProperties,
        IServiceProvider serviceProvider,
        IVersionProvider versionProvider,
        IDbContextFactory<DatabaseContext> dbFactory
    )
    {
        _logger = logger;
        _bot = botProperties.Bot;
        _serviceProvider = serviceProvider;
        _versionProvider = versionProvider;
        _dbFactory = dbFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var databaseContext = await _dbFactory.CreateDbContextAsync(stoppingToken);
        {
            await databaseContext.Database.MigrateAsync(cancellationToken: stoppingToken);
        }

        _logger.LogInformation($"Запуск бота [{_versionProvider.FileVersionInfo.FileVersion}]");
        int? offset = null;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var updates = offset.HasValue
                    ? (await _bot.GetUpdatesAsync(offset.Value, cancellationToken: stoppingToken).ConfigureAwait(false))
                        .ToArray()
                    : (await _bot.GetUpdatesAsync(cancellationToken: stoppingToken).ConfigureAwait(false)).ToArray();

                if (updates.Length == 0)
                {
                    continue;
                }

                await Parallel.ForEachAsync(
                    updates,
                    stoppingToken,
                    async (update, token) =>
                    {
                        try
                        {
                            await ProcessUpdate(update, token).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Ошибка обработки update {UpdateId}", update.UpdateId);
                        }
                    });

                offset = updates[^1].UpdateId + 1;
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogWarning(exception, "Ошибка сети при получении updates от Telegram, повтор через {DelaySec} сек", RetryDelay.TotalSeconds);
                await Task.Delay(RetryDelay, stoppingToken);
            }
            catch (SocketException exception)
            {
                _logger.LogWarning(exception, "Сокетная ошибка при работе с Telegram API, повтор через {DelaySec} сек", RetryDelay.TotalSeconds);
                await Task.Delay(RetryDelay, stoppingToken);
            }
            catch (BotRequestException exception)
            {
                _logger.LogError(exception, "Ошибка Telegram API");
                await Task.Delay(RetryDelay, stoppingToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Неожиданная ошибка в цикле обработки обновлений, повтор через {DelaySec} сек", RetryDelay.TotalSeconds);
                try
                {
                    await Task.Delay(RetryDelay, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
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
        _logger.LogInformation($"Worker stopping at: {DateTimeOffset.Now}");
        return base.StopAsync(cancellationToken);
    }
}
