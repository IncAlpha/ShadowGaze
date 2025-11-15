using System.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Core.Models.SessionContexts;
using ShadowGaze.Core.Services.Extensions;
using ShadowGaze.Core.Services.QrCodes;
using ShadowGaze.Data.Models.Database;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.CallbackQueries.Endpoints;

public class EndpointsProcessor(
    ILogger<EndpointsProcessor> logger,
    PublicBotProperties botProperties,
    CustomersRepository customersRepository,
    InboundRepository inboundRepository,
    ConnectionsRepository connectionsRepository
) : BaseUpdateProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, _) =>
        type is UpdateTypes.CallbackQuery && update is { CallbackQuery.Data: CallbackQueriesConstants.Endpoints };
    
    public override async Task Process(Update update, SessionContext sessionContext)
    {
        var query = update.CallbackQuery!;
        var message = query.Message!;
        var chatId = message.Chat.Id;
        var user = query.From;
        await Bot.AnswerCallbackQueryAsync(new AnswerCallbackQueryArgs(query.Id));
        
        var username = string.IsNullOrWhiteSpace(user.Username) ? chatId.ToString() : user.Username;
        var customer = await customersRepository.GetOrCreateAsync(user.Id, username);
        var connections = await GetOrCreateConnectionsAsync(customer);
        
        foreach (var connection in connections)
        {
            var qrCodeArgs = GetQrCodeMessageArgs(chatId, connection.ConnectionString);
            await Bot.SendPhotoAsync(qrCodeArgs);
        }
        var messageArgs = new SendMessageArgs(chatId, "Следуйте инструкциям по использованию")
        {
            ChatId = chatId,
            ReplyMarkup = GetKeyboard()
        };
        await Bot.SendMessageAsync(messageArgs);
        
        foreach (var connection in connections)
        {
            await connectionsRepository.SaveAsync(connection);
        }
    }
    
    private async Task<List<Connection>> GetOrCreateConnectionsAsync(Customer customer)
    {
        var connections = await connectionsRepository.AsQueryable().ToListAsync();
        if (connections.Count != 0)
        {
            return connections;
        }
        var inbounds = await inboundRepository
            .AsQueryable()
            .Where(i => !i.Obsolete)
            .ToListAsync();
        return inbounds
            .Select(inbound => BuildConnection(inbound, customer))
            .ToList();
    }
    
    private InlineKeyboardMarkup GetKeyboard()
    {
        return BuildKeyboard()
            .AppendCallbackData("Инструкции", CallbackQueriesConstants.Instructions)
            .AppendRow()
            .AppendCallbackData("Главное меню", CallbackQueriesConstants.MainMenu)
            .Build();
    }

    private Connection BuildConnection(Inbound inbound, Customer customer)
    {
        var guid = Guid.NewGuid();
        return new Connection
        {
            CustomerId = customer.Id,
            VlessInboundId = inbound.Id,
            ClientId = guid,
            ConnectionString = BuildConnectionString(guid, inbound),
            CreatedAt = DateTime.Now,
            ExpiryDate = DateTime.Now.AddDays(21).Date
        };
    }

    private string BuildConnectionString(Guid clientId, Inbound inbound)
    {
        var protocol = inbound.Protocol;
        var host = $"{inbound.Address}:{inbound.Port}";

        var queryParams = HttpUtility.ParseQueryString(string.Empty);
        queryParams.Add("encryption", "none");
        queryParams.Add("flow", inbound.Flow);
        queryParams.Add("type", inbound.Network);
        queryParams.Add("security", inbound.Security);
        queryParams.Add("sni", inbound.ServerName);
        queryParams.Add("pbk", inbound.PublicKey);
        queryParams.Add("sid", inbound.ShortId);

        return $"{protocol}://{clientId}@{host}?{queryParams}#{inbound.ConnectionTag}";    
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