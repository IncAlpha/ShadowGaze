using Google.Protobuf;
using Grpc.Net.Client;
using Xray.App.Proxyman.Command;
using Xray.Common.Protocol;
using Xray.Common.Serial;
using Xray.Proxy.Vless;

namespace ShadowGaze.Core.Services.XRay;

public class XraySync
{
    private readonly HandlerService.HandlerServiceClient _client;
    private readonly GrpcChannel _channel;

    public XraySync()
    {
        _channel = GrpcChannel.ForAddress("http://127.0.0.1:10085");
        _client = new HandlerService.HandlerServiceClient(_channel);
    }

    public async Task GetInboundConfiguration(string inboundTag)
    {
        var inbound = await _client.ListInboundsAsync(new ListInboundsRequest());
        var res = await _client.GetInboundUsersAsync(new GetInboundUserRequest
        {
            Tag = inboundTag,
        });
        var users = res.Users.ToList();
        var account = Account.Parser.ParseFrom(res.Users[0].Account.Value);
    }

    public async Task AddClient(string inboundTag, string email)
    {
        var account = new Account
        {
            Id = Guid.NewGuid().ToString(),
            Flow = "xtls-rprx-vision",
        };
        
        var user = new User
        {
            Email = email,
            Level = 0,
            Account = new TypedMessage
            {
                Type = "xray.proxy.vless.Account",
                Value =  account.ToByteString()    
            }
        };
        
        var operation = new AddUserOperation()
        {
            User = user
        };
        var request = new AlterInboundRequest
        {
            Tag = inboundTag,
            Operation = new TypedMessage
            {
                Type = "xray.app.proxyman.command.AddUserOperation",
                Value = operation.ToByteString()   
            }
        };
        await _client.AlterInboundAsync(request);
        Console.WriteLine("✅ Пользователь добавлен!");
    }

    public async Task RemoveClient(string inboundTag, string email)
    {
        var operation = new RemoveUserOperation
        {
            Email = email
        };
        var request = new AlterInboundRequest
        {
            Tag = inboundTag,
            Operation = new TypedMessage
            {
                Type = "xray.app.proxyman.command.RemoveUserOperation",
                Value = operation.ToByteString()   
            }
        };
        await _client.AlterInboundAsync(request);
        Console.WriteLine("✅ Пользователь удален!");
    }

    public async Task CheckAlive()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        try
        {
            await _channel.ConnectAsync(cts.Token);
            Console.WriteLine($"✅ gRPC сервер доступен ({_channel.State})");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка подключения: {ex.Message}");
        }
    }
}