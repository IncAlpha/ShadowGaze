using BigBroPublicBot.Core.Models;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;

namespace BigBroPublicBot.Core.Services;

public class BotBackgroundService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly BotClient _api;

    public BotBackgroundService(ILogger<BotBackgroundService> logger, 
        PublicBotProperties botProperties, 
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _api = botProperties.Api;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);

        var updates = await _api.GetUpdatesAsync(cancellationToken: stoppingToken).ConfigureAwait(false);
        while (!stoppingToken.IsCancellationRequested)
        {
            if (updates.Any())
            {
                Parallel.ForEach(updates, ProcessUpdate);

                updates = await _api.GetUpdatesAsync(updates[^1].UpdateId + 1, cancellationToken: stoppingToken)
                    .ConfigureAwait(false);
            }
            else
            {
                updates = await _api.GetUpdatesAsync(cancellationToken: stoppingToken).ConfigureAwait(false);
            }
        }
    }

    private void ProcessUpdate(Update update)
    {
        using var scope = _serviceProvider.CreateScope();
        var bot = scope.ServiceProvider.GetRequiredService<PublicBot>();
        bot.OnUpdate(update);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Worker stopping at: {Time}", DateTimeOffset.Now);
        return base.StopAsync(cancellationToken);
    }
}