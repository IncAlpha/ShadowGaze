using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Core.Models.SessionContexts;
using ShadowGaze.Core.Services.Xray;
using ShadowGaze.Core.Services.Xray.Messages;
using ShadowGaze.Data.Models.Database;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.Messages.AdminCommands.XrayCore;

public class SyncXrayProcessor(
    ILogger<SyncXrayProcessor> logger,
    PublicBotProperties botProperties,
    InboundRepository inboundRepository,
    IXrayClientFactory xrayClientFactory,
    CustomersRepository customersRepository,
    ConnectionsRepository connectionsRepository
) : BaseUpdateProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, _) =>
        type is UpdateTypes.Message && update.Message?.Text == AdminCommandsConstants.Xray;

    public override async Task Process(Update update, SessionContext sessionContext)
    {
        var localUsers = await customersRepository
            .AsQueryable()
            .Where(e => e.ExpiryDate > DateTime.Now)
            .ToListAsync();

        var inbounds = await inboundRepository
            .AsQueryable()
            .Where(i => i.Obsolete == false)
            .ToListAsync();

        var syncTasks = inbounds.Select(inbound => SyncXrayAsync(localUsers, inbound))
            .ToArray();
        await Task.WhenAll(syncTasks);
        logger.LogInformation("Xrays synchronized ");
    }

    private async Task SyncXrayAsync(List<Customer> localEndpoints, Inbound inbound)
    {
        var client = xrayClientFactory.GetClient(new Uri($"{inbound.ApiUri}"));
        var message = new GetInboundUsersMessage(inbound.InboundTag);
        using var timeoutSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var remoteEndpoint = await client.GetData(message, timeoutSource.Token);
        var localConnections = await connectionsRepository
            .AsQueryable()
            .Where(c => c.VlessInboundId == inbound.Id)
            .ToListAsync();
        
        // список подключений на пользователей
        var toAdd = localConnections
            .Where(l => remoteEndpoint.All(r => r.Id != l.ClientId))
            .ToList();
        var toRemove = remoteEndpoint
            .Where(r => localConnections.All(l => l.ClientId != r.Id))
            .ToList();

        foreach (var item in toAdd)
        {
            var addUser = new AddUserMessage(inbound.InboundTag, item.Email, item.ClientId);
            await client.SendMessage(addUser, timeoutSource.Token);
            logger.LogInformation($"Adding X-ray user {item.Email} at {inbound.ApiUri}");
        }

        foreach (var item in toRemove)
        {
            // var deleteUser = new RemoveUserMessage(inboundTag, item.Email);
            // await client.SendMessage(deleteUser);
            logger.LogInformation($"Remove X-ray user {item.Email} at {inbound.ApiUri}");
        }
    }
}