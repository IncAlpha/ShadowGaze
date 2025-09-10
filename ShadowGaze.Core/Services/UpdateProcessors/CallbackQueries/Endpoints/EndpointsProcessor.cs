using System.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Congifurations;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Core.Models.SessionContexts;
using ShadowGaze.Core.Models.XUI;
using ShadowGaze.Core.Services.Extensions;
using ShadowGaze.Core.Services.QrCodes;
using ShadowGaze.Core.Services.XUI;
using ShadowGaze.Data.Models.Database;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.UpdatingMessages;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.CallbackQueries.Endpoints;

public class EndpointsProcessor(
    ILogger<EndpointsProcessor> logger,
    IOptions<XUiOptions> options,
    PublicBotProperties botProperties,
    CustomersRepository customersRepository,
    EndpointsRepository endpointsRepository,
    XrayRepository xrayRepository,
    XuiService xuiService
) : BaseUpdateProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, _) =>
        type is UpdateTypes.CallbackQuery && update is { CallbackQuery.Data: CallbackQueriesConstants.Endpoints };

    private readonly XUiOptions _options = options.Value;

    public override async Task Process(Update update, SessionContext sessionContext)
    {
        var query = update.CallbackQuery!;
        var message = query.Message!;
        var chatId = message.Chat.Id;
        var user = query.From;
        await Bot.AnswerCallbackQueryAsync(new AnswerCallbackQueryArgs(query.Id));
        
        var customer = await customersRepository.GetOrCreateAsync(user.Id, user.Username);
        var endpoint = await GetOrCreateEndpointAsync(customer);
        if (endpoint == null)
        {
            await Bot.EditMessageTextAsync(chatId, message.MessageId,
                "Произошла внутренняя ошибка, обратитесь к специалисту технической поддержки");
            return;
        }
        var connectionString = await GetOrCreateConnectionStringAsync(endpoint, user);
        if (connectionString == null)
        {
            await Bot.EditMessageTextAsync(chatId, message.MessageId,
                "Произошла внутренняя ошибка, обратитесь к специалисту технической поддержки");
            return;
        }
        
        var qrCodeArgs = GetQrCodeMessageArgs(chatId, connectionString);
        await Bot.SendPhotoAsync(qrCodeArgs);
        var messageArgs = new SendMessageArgs(chatId, "Следуйте инструкциям по использованию")
        {
            ChatId = chatId,
            ReplyMarkup = GetKeyboard()
        };
        await Bot.SendMessageAsync(messageArgs);
    }

    private async Task<Endpoint> GetOrCreateEndpointAsync(Customer customer)
    {
        if (customer.EndpointId is null)
        {
            // TODO: проверять на наличие клиента
            Client newClient;
            try
            { 
                newClient = await xuiService.AddDefaultClient(_options.XUiConfigurationId, _options.InboundId, customer.TelegramName);
            }
            catch (Exception e)
            {
                logger.LogError("Не удалось создать клиента для inbound");
                return null;
            }

            var endpoint = new Endpoint
            {
                XrayId = _options.XUiConfigurationId,
                InboundId = _options.InboundId,
                ClientId = newClient.Id,
                CreatedAt = DateTime.Now,
                ExpiryDate = newClient.ExpiryTime
            };
            await endpointsRepository.SaveAsync(endpoint);
            customer.EndpointId = endpoint.Id;
            await customersRepository.SaveAsync(customer);
            return endpoint;
        }
        return await endpointsRepository.GetByIdAsync(customer.EndpointId!.Value);
    }

    private async Task<string> GetOrCreateConnectionStringAsync(Endpoint endpoint, User user)
    {
        if (endpoint.ConnectionString is null)
        {
            var xray = await xrayRepository.GetByIdAsync(endpoint.XrayId);
            InboundDto inboundDto;
            try
            {
                var inbound = await xuiService.GetInbound(endpoint.XrayId, _options.InboundId);
                inboundDto = inbound.Object;

            }
            catch (Exception e)
            {
                logger.LogError("Не удалось получить inbound");
                return null;
            }
            var connectionString = BuildConnectionString(user, endpoint, xray, inboundDto);
            endpoint.ConnectionString = connectionString;
            await endpointsRepository.SaveAsync(endpoint);    
        }
        return endpoint.ConnectionString;
    }

    private InlineKeyboardMarkup GetKeyboard()
    {
        return BuildKeyboard()
            .AppendCallbackData("Инструкции", CallbackQueriesConstants.Instructions)
            .AppendRow()
            .AppendCallbackData("Главное меню", CallbackQueriesConstants.MainMenu)
            .Build();
    }

    private string BuildConnectionString(User user, Endpoint endpoint, Xray xray, InboundDto inbound)
    {
        var protocol = inbound.Protocol;
        var client = inbound.Settings.Clients.FirstOrDefault(client => client.Id == endpoint.ClientId.ToString())!;
        var id = client.Id;
        var host = $"{xray.Host}:443";
        var flow = client.Flow;

        var queryParams = HttpUtility.ParseQueryString(string.Empty);
        queryParams.Add("type", inbound.StreamSettings.Network);
        queryParams.Add("security", inbound.StreamSettings.Security);
        queryParams.Add("pbk", inbound.StreamSettings.RealitySettings.Settings.PublicKey);
        queryParams.Add("fp", inbound.StreamSettings.RealitySettings.Settings.Fingerprint);
        queryParams.Add("sni", inbound.StreamSettings.RealitySettings.ServerNames[0]);
        queryParams.Add("sid", inbound.StreamSettings.RealitySettings.ShortIds[0]);
        queryParams.Add("spx", inbound.StreamSettings.RealitySettings.Settings.SpiderX);
        queryParams.Add("flow", flow);

        return $"{protocol}://{id}@{host}?{queryParams}#sg-{user.Username}";
    }

    private SendPhotoArgs GetQrCodeMessageArgs(long chatId, string qrCodeContent)
    {
        var ms = new MemoryStream(QrCodeGenerator.GetQrImage(qrCodeContent), writable: false);
        var input = new InputFile(ms, "test");
        return new SendPhotoArgs(chatId, input)
        {
            Caption = $"`{qrCodeContent}`",
            ParseMode = "MarkdownV2"
        };
    }
}