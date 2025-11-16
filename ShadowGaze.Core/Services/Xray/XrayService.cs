using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShadowGaze.Core.Services.UpdateProcessors.Messages.AdminCommands.XrayCore;
using ShadowGaze.Core.Services.Xray.Messages;
using ShadowGaze.Data.Models.Database;
using ShadowGaze.Data.Services.Database;

namespace ShadowGaze.Core.Services.Xray;

public class XrayService(
    ILogger<SyncXrayProcessor> logger,
    IXrayClientFactory xrayClientFactory,
    InboundRepository inboundRepository,
    CustomersRepository customersRepository)
{
    public async Task AddUser(Customer user)
    {
        var inbounds = await inboundRepository
            .AsQueryable()
            .Where(i => i.Obsolete == false)
            .ToListAsync();
        
        var syncTasks = inbounds.Select(inbound => AddUserAsync(user, inbound))
            .ToArray();
        await Task.WhenAll(syncTasks);
        logger.LogInformation("Xrays synchronized ");
    }

    private async Task AddUserAsync(Customer user, Inbound inbound)
    {
        var client = xrayClientFactory.GetClient(new Uri($"{inbound.ApiUri}"));
        var message = new GetInboundUsersMessage(inbound.InboundTag);
        using var timeoutSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var remoteEndpoint = await client.GetData(message, timeoutSource.Token);

        if (remoteEndpoint.Any(u => u.Id == user.ClientId))
        {
            return;
        }
        var addUser = new AddUserMessage(inbound.InboundTag, user.TelegramName, user.ClientId);
        await client.SendMessage(addUser, timeoutSource.Token);
        logger.LogInformation($"Adding X-ray user {user.TelegramName} at {inbound.ApiUri}");
    }

    public async Task SyncAsync()
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
    
    private async Task SyncXrayAsync(List<Customer> localUsers, Inbound inbound)
    {
        var client = xrayClientFactory.GetClient(new Uri($"{inbound.ApiUri}"));
        var message = new GetInboundUsersMessage(inbound.InboundTag);
        using var timeoutSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var remoteEndpoint = await client.GetData(message, timeoutSource.Token);
        
        // список подключений на пользователей
        var toAdd = localUsers
            .Where(l => remoteEndpoint.All(r => r.Id != l.ClientId))
            .ToList();
        var toRemove = remoteEndpoint
            .Where(r => localUsers.All(l => l.ClientId != r.Id))
            .ToList();

        foreach (var item in toAdd)
        {
            var addUser = new AddUserMessage(inbound.InboundTag, item.TelegramName, item.ClientId);
            await client.SendMessage(addUser, timeoutSource.Token);
            logger.LogInformation($"Adding X-ray user {item.TelegramName} at {inbound.ApiUri}");
        }

        foreach (var item in toRemove)
        {
            // var deleteUser = new RemoveUserMessage(inboundTag, item.Email);
            // await client.SendMessage(deleteUser);
            logger.LogInformation($"Remove X-ray user {item.Email} at {inbound.ApiUri}");
        }
    }
}