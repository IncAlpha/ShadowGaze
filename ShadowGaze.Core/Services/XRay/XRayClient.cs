using Grpc.Net.Client;
using ShadowGaze.Core.Services.XRay.Messages;
using Xray.App.Proxyman.Command;

namespace ShadowGaze.Core.Services.XRay;

public class XRayClient : IXRayClient, ICheckAlive
{
    private readonly HandlerService.HandlerServiceClient _client;
    private readonly GrpcChannel _channel;
    
    public XRayClient(int port)
    {
        _channel = GrpcChannel.ForAddress($"http://127.0.0.1:{port}");
        _client = new HandlerService.HandlerServiceClient(_channel);
    }
    
    public XRayClient(Uri address)
    {
        _channel = GrpcChannel.ForAddress(address);
        _client = new HandlerService.HandlerServiceClient(_channel);
    }

    public async Task SendMessage(IXrayClientMessage message)
    {
        try
        {
            await message.SendMessage(_client);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<T> GetData<T>(IXrayClientMessage<T> message)
    {
        return await message.GetData(_client);
    }

    public async Task<bool> CheckAliveAsync()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        try
        {
            await _channel.ConnectAsync(cts.Token);
            Console.WriteLine($"✅ gRPC сервер доступен ({_channel.State})");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка подключения: {ex.Message}");
            return false;
        }
    }
}


public interface IXrayClientFactory
{
    public IXRayClient GetClient(int port);
}
public class XrayClientFactory: IXrayClientFactory
{
    private readonly Dictionary<int, IXRayClient> _clients = new();
    
    public IXRayClient GetClient(int port)
    {
        if (_clients.TryGetValue(port, out var client))
        {
            return client;
        }
        var newClient = new XRayClient(port);
        _clients.Add(port, newClient);
        return newClient;
    }
}