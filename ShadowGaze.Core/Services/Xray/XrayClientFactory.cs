using System.Collections.Concurrent;

namespace ShadowGaze.Core.Services.Xray;

public class XrayClientFactory: IXrayClientFactory
{
    private readonly ConcurrentDictionary<Uri, IXrayClient> _clients = new();
    
    public IXrayClient GetClient(Uri uri)
    {
        if (_clients.TryGetValue(uri, out var client))
        {
            return client;
        }
        var newClient = new XrayClient(uri);
        _clients.TryAdd(uri, newClient);
        return newClient;
    }
}