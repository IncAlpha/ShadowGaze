using System.Web;
using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Core.Services.QrCodes;
using ShadowGaze.Data.Models.Database;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.CallbackQueries.Connections;

public abstract class SendConnectionProcessor(
    ConnectionsRepository connectionsRepository,
    PublicBotProperties botProperties
) : BaseUpdateProcessor(botProperties)
{
    protected async Task SendMessage(long chatId, Inbound inbound, Customer customer)
    {
        var connection = await connectionsRepository.GetByInboundId(inbound.Id) ?? BuildConnection(inbound, customer);
        var qrCodeArgs = GetQrCodeMessageArgs(chatId, connection.ConnectionString);
        await Bot.SendPhotoAsync(qrCodeArgs);
        
        var messageArgs = new SendMessageArgs(chatId, "Следуйте инструкциям по использованию")
        {
            ChatId = chatId,
            ReplyMarkup = GetKeyboard()
        };
        await Bot.SendMessageAsync(messageArgs);
        await connectionsRepository.SaveAsync(connection);    
    }
    
    private InlineKeyboardMarkup GetKeyboard()
    {
        return BuildKeyboard()
            .AppendCallbackData("Инструкции", CallbackQueriesConstants.Instructions)
            .AppendRow()
            .AppendCallbackData("Все страны", CallbackQueriesConstants.ConnectionsList)
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
            Email = customer.TelegramName ?? guid.ToString(),
            ConnectionString = BuildConnectionString(guid, inbound)
        };
    }

    private string BuildConnectionString(Guid clientId, Inbound inbound)
    {
        var protocol = inbound.Protocol;

        var queryParams = HttpUtility.ParseQueryString(string.Empty);
        queryParams.Add("encryption", "none");
        queryParams.Add("flow", inbound.Flow);
        queryParams.Add("type", inbound.Network);
        queryParams.Add("security", inbound.Security);
        queryParams.Add("sni", inbound.ServerName);
        queryParams.Add("pbk", inbound.PublicKey);
        queryParams.Add("sid", inbound.ShortId);

        return $"{protocol}://{clientId}@{inbound.ConnectionUri}?{queryParams}#{inbound.ConnectionName}";    
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