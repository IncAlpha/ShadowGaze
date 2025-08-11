using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ShadowGaze.Core.Models;
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

    public BaseMessageProcessor(ILogger<BaseMessageProcessor> logger,
        HttpClient httpClient, IConfiguration configuration)
    {
        _logger = logger;
        _httpClient = httpClient;
        _secretConfigurationSection = configuration.GetSection("secret");
    }

    public override async Task Process(Update update)
    {
        var body = new List<KeyValuePair<string, string>>
        {
            new("username", _secretConfigurationSection["3xui_login"]),
            new("password", _secretConfigurationSection["3xui_password"])
        };
        
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://alpha.bigbro.group/alphaproxypanel/login"),
            Content = new FormUrlEncodedContent(body)
        };
        
        var response = await _httpClient.SendAsync(request);
        _logger.LogInformation($"Код ответа: {response.StatusCode}");
        _logger.LogInformation($"Тело ответа: {await response.Content.ReadAsStringAsync()}");
    }
}