using Grpc.Net.Client;
using ShadowGaze.Core.Services.Xray.Messages;
using Xray.App.Proxyman.Command;

namespace ShadowGaze.Core.Services.Xray;

public class XrayClient : IXrayClient, ICheckAlive
{
    private readonly HandlerService.HandlerServiceClient _client;
    private readonly GrpcChannel _channel;
    
    public XrayClient(Uri uri)
    {
        _channel = GrpcChannel.ForAddress(uri);
        _client = new HandlerService.HandlerServiceClient(_channel);
    }

    public async Task SendMessage(IXrayClientMessage message, CancellationToken token)
    {
        try
        {
            await message.SendMessage(_client, token);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<T> GetData<T>(IXrayClientMessage<T> message, CancellationToken token)
    {
        return await message.GetData(_client, token);
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