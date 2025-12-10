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
    protected async Task SendMessage(long chatId, Connection connection)
    {
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

    protected Connection BuildConnection(ConnectionConfiguration connectionConfiguration, Customer customer)
    {
        return new Connection
        {
            CustomerId = customer.Id,
            ConnectionConfigurationId = connectionConfiguration.Id,
            ConnectionString = BuildConnectionString(customer.ClientId, connectionConfiguration)
        };
    }

    private string BuildConnectionString(Guid clientId, ConnectionConfiguration connectionConfiguration)
    {
        var protocol = connectionConfiguration.Protocol;

        var queryParams = HttpUtility.ParseQueryString(string.Empty);
        queryParams.Add("encryption", "none");
        queryParams.Add("flow", connectionConfiguration.Flow);
        queryParams.Add("type", connectionConfiguration.Network);
        queryParams.Add("security", connectionConfiguration.Security);
        queryParams.Add("sni", connectionConfiguration.ServerName);
        queryParams.Add("pbk", connectionConfiguration.PublicKey);
        queryParams.Add("sid", connectionConfiguration.ShortId);

        return $"{protocol}://{clientId}@{connectionConfiguration.ConnectionUri}?{queryParams}#{connectionConfiguration.ConnectionName}";    
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