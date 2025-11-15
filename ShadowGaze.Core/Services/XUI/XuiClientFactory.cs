using System.Collections.Concurrent;
using System.Net;
using Microsoft.Extensions.Logging;

namespace ShadowGaze.Core.Services.XUI;

public class XuiClientFactory(
    IHttpClientFactory factory,
    ILogger<XuiApiClient> logger,
    CookieContainer cookieContainer)
    : IXuiClientFactory
{
    private readonly ConcurrentDictionary<int, IXuiApiClient> _clients = new();

    public IXuiApiClient GetClient(int xrayId)
    {
        return _clients.GetOrAdd(xrayId, id =>
        {
            // var xray = xrayRepository.GetById(id);
            var xray = new Data.Models.Database.Xray();
            var httpClient = factory.CreateClient();
            httpClient.BaseAddress = new Uri($"https://{xray.Host}:{xray.Port}/{xray.Path}/");
            return new XuiApiClient(logger, httpClient, cookieContainer, xray);
        });
    }
}