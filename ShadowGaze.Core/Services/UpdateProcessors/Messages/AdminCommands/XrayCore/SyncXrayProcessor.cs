using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Core.Models.SessionContexts;
using ShadowGaze.Core.Services.XRay;
using ShadowGaze.Core.Services.XRay.Messages;
using ShadowGaze.Data.Models.Database;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.Messages.AdminCommands.XrayCore;

public class SyncXrayProcessor(
    PublicBotProperties botProperties, 
    InboundRepository inboundRepository, 
    IXrayClientFactory xrayClientFactory,
    CustomersRepository customersRepository) : BaseUpdateProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, _) =>
        type is UpdateTypes.Message && update.Message?.Text == AdminCommandsConstants.Xray;

    public override async Task Process(Update update, SessionContext sessionContext)
    {
        var localUsers = await customersRepository
            .AsQueryable()
            .Include(c => c.Endpoint)
            .Where(e => e.Endpoint.ExpiryDate > DateTime.Now)
            .ToListAsync();
        
        var inbounds = inboundRepository
            .AsQueryable()
            .Where(i => i.Obsolete == false)
            .ToList();
        
        var syncTasks = inbounds.Select(async inbound =>
        {
            await SyncXrayAsync(localUsers, inbound.TunnelPort, inbound.InboundTag);
        });
        await Task.WhenAll(syncTasks);
        Console.WriteLine("Xrays synchronized ");
        
        // var inbounds = inboundRepository.AsQueryable().Where(i => i.Obsolete == false);
        // foreach (var inbound in inbounds)
        // {
        //     var client = xrayClientFactory.GetClient(inbound.TunnelPort);
        //     await client.SendMessage(new GetInbounds());
        // }


        // var message = new GetInbounds();
        // await client.SendMessage(new GetInbounds());
        // await xraySyncService.SynchronizeUsersAsync();
        // var message = new AddUserMessage("vless-in", "repinss@sg.group");
        // await xRayClient.SendMessage(message);
        // await xraySync.CheckAlive();
        // await xraySync.GetInboundConfiguration("vless-in");
        // await xraySync.AddClient("vless-in", "repinss");
        // await xraySync.RemoveClient("vless-in", "newuser@example.com");
    }
    
    private async Task SyncXrayAsync(List<Customer> localEndpoints, int port, string inboundTag)
    {
        var client = xrayClientFactory.GetClient(port);
        var message = new GetInboundUsersMessage(inboundTag);
        List<XrayUser> remoteEndpoint;
        try
        {
             remoteEndpoint = await client.GetData(message);
            
        }
        catch (RpcException e)
        {
            Console.WriteLine($"Не удалось выполнить запрос к RPC {port} с ошибкой\n {e.Message}");
            return;
        }
        
        var toAdd = localEndpoints
            .Where(l => remoteEndpoint.All(r => r.Id != l.Endpoint.ClientId))
            .ToList();
        var toRemove = remoteEndpoint
            .Where(r => localEndpoints.All(l => l.Endpoint.ClientId != r.Id))
            .ToList();
        
        try
        {
            foreach (var item in toAdd)
            {
                var addUser = new AddUserMessage(inboundTag, item.TelegramName, item.Endpoint.ClientId);
                await client.SendMessage(addUser);
                Console.WriteLine($"Adding X-ray user {item.TelegramName} on port {port}");
            }
        }
        catch (RpcException e)
        {
            Console.WriteLine($"Не удалось выполнить запрос к RPC {port} с ошибкой\n {e.Message}");
        }

        try
        {
            foreach (var item in toRemove)
            {
                // var deleteUser = new RemoveUserMessage(inboundTag, item.Email);
                // await client.SendMessage(deleteUser);
                Console.WriteLine($"Remote X-ray user {item.Email} on port {port}");
            }
        }
        catch (RpcException e)
        {
            Console.WriteLine($"Не удалось выполнить запрос к RPC {port} с ошибкой\n {e.Message}");
        }
    }
}

// public class XraySyncService(CustomersRepository customersRepository, IXrayClientFactory xrayClientFactory)
// {
//     private const string InboundTag = "vless-in";
//     public async Task SynchronizeUsersAsync()
//     {
//         var localUsers = await customersRepository
//             .AsQueryable()
//             .Include(c => c.Endpoint)
//             .Where(e => e.Endpoint.ExpiryDate > DateTime.Now)
//             .ToListAsync();
//
//         var ports = XrayPorts.Get();
//         var syncTasks = ports.Select(async port =>
//         {
//             await SyncXrayAsync(localUsers, port);
//         });
//         await Task.WhenAll(syncTasks);
//         Console.WriteLine("Synchronized Xrays");
//     }
//
//     private async Task SyncXrayAsync(List<Customer> localEndpoints, int port)
//     {
//         var client = xrayClientFactory.GetClient(port);
//         var message = new GetInboundUsersMessage(InboundTag);
//         List<XrayUser> remoteEndpoint;
//         try
//         {
//              remoteEndpoint = await client.GetData(message);
//             
//         }
//         catch (RpcException e)
//         {
//             Console.WriteLine($"Не удалось выполнить запрос к RPC {port} с ошибкой\n {e.Message}");
//             return;
//         }
//         
//         var toAdd = localEndpoints
//             .Where(l => remoteEndpoint.All(r => r.Id != l.Endpoint.ClientId))
//             .ToList();
//         var toRemove = remoteEndpoint
//             .Where(r => localEndpoints.All(l => l.Endpoint.ClientId != r.Id))
//             .ToList();
//         
//         try
//         {
//             foreach (var item in toAdd)
//             {
//                 var addUser = new AddUserMessage(InboundTag, item.TelegramName, item.Endpoint.ClientId);
//                 await client.SendMessage(addUser);
//                 Console.WriteLine($"Adding X-ray user {item.TelegramName} on port {port}");
//             }
//         }
//         catch (RpcException e)
//         {
//             Console.WriteLine($"Не удалось выполнить запрос к RPC {port} с ошибкой\n {e.Message}");
//         }
//
//         try
//         {
//             foreach (var item in toRemove)
//             {
//                 var deleteUser = new RemoveUserMessage(InboundTag, item.Email);
//                 await client.SendMessage(deleteUser);
//                 Console.WriteLine($"Remote X-ray user {item.Email} on port {port}");
//             }
//         }
//         catch (RpcException e)
//         {
//             Console.WriteLine($"Не удалось выполнить запрос к RPC {port} с ошибкой\n {e.Message}");
//         }
//     }
// }

// public static class XrayPorts
// {
//     public static List<int> Get() => [10085];
// }