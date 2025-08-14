using ShadowGaze.Core.Models;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.UpdatingMessages;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.CallbackQuery;

public class GetLinkCallbackQueryProcessor(PublicBotProperties botProperties, CustomersRepository customersRepository): BaseCallbackQueryProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, context) =>
        type is UpdateTypes.CallbackQuery && update.CallbackQuery?.Data == "main;get_links";

    public override async Task Process(Update update)
    {
        var query = update.CallbackQuery;
        await Api.AnswerCallbackQueryAsync(new AnswerCallbackQueryArgs(query.Id));
        
        var user = update.CallbackQuery?.From;
        if (user == null)
        {
            return;
        }
        var customer = await customersRepository.GetOrCreateAsync(user.Id, user.Username);
        if (customer.EndpointId != null)
        {
            await EditMessageGetEndpoint(query, (int)customer.EndpointId);
        }
        else
        {
            await EditMessageCreateEndpoint(query);    
        }
    }

    private async Task EditMessageGetEndpoint(Telegram.BotAPI.AvailableTypes.CallbackQuery query, int endpointId)
    {
        var builder = new InlineKeyboardBuilder();
        builder.AppendCallbackData("Текстовая ссылка", $"get_endpoint;text;{endpointId}");
        builder.AppendCallbackData("QR code", $"get_endpoint;qrcode{endpointId}");
        await Api.EditMessageTextAsync<Message>(
            new EditMessageTextArgs("Выберите удобный способ")
            {
                ChatId = query.Message.Chat.Id,
                MessageId = query.Message.MessageId,
                ReplyMarkup = new InlineKeyboardMarkup(builder)
            });
    }

    private async Task EditMessageCreateEndpoint(Telegram.BotAPI.AvailableTypes.CallbackQuery query)
    {
        await Api.EditMessageTextAsync(query.Message.Chat.Id, query.Message.MessageId,
            "Вам доступен пробный период: Ссылка для подключения появится в боте в течении часа");
        await Api.SendMessageAsync(-4929973929,
            $"Username: {query.Message.Chat.Username} ID: {query.Message.Chat.Id} хочет прокси");
    }
}