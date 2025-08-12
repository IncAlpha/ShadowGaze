using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ShadowGaze.Core.Models;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.Messages;

public class BaseMessageProcessor : BaseUpdateProcessor
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, context) =>
        type is UpdateTypes.Message && update.Message?.Text == "login";

    private readonly ILogger<BaseMessageProcessor> _logger;
    private readonly HttpClient _httpClient;
    private readonly IConfigurationSection _secretConfigurationSection;
    private readonly XrayRepository _xrayRepository;

    public BaseMessageProcessor(
        ILogger<BaseMessageProcessor> logger,
        HttpClient httpClient, 
        IConfiguration configuration,
        XrayRepository xrayRepository)
    {
        _logger = logger;
        _httpClient = httpClient;
        _secretConfigurationSection = configuration.GetSection("secret");
        _xrayRepository = xrayRepository;
    }

    public override async Task Process(Update update)
    {
        var xray = await _xrayRepository.GetByIdAsync(1);
        
        var body = new List<KeyValuePair<string, string>>
        {
            new("username", xray.Username),
            new("password", xray.Password)
        };
        var uriBuilder = new UriBuilder
        {
            Scheme = Uri.UriSchemeHttps,
            Host = xray.Host,
            Port = xray.Port,
            Path = "alphaproxypanel/login"
        };
        
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = uriBuilder.Uri,
            Content = new FormUrlEncodedContent(body)
        };
        
        var response = await _httpClient.SendAsync(request);
        _logger.LogInformation($"Код ответа: {response.StatusCode}");
        _logger.LogInformation($"Тело ответа: {await response.Content.ReadAsStringAsync()}");
    }
}