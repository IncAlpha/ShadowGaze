using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ShadowGaze.Core.Models;
using ShadowGaze.Core.Services.XUI;
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
    private readonly AuthService _authService;

    public BaseMessageProcessor(
        ILogger<BaseMessageProcessor> logger,
        HttpClient httpClient, 
        IConfiguration configuration,
        XrayRepository xrayRepository,
        AuthService  authService)
    {
        _logger = logger;
        _httpClient = httpClient;
        _secretConfigurationSection = configuration.GetSection("secret");
        _xrayRepository = xrayRepository;
        _authService = authService;
    }

    public override async Task Process(Update update)
    {
        var xray = await _xrayRepository.GetByIdAsync(1);
        await _authService.LoginAsync(xray);
    }
}