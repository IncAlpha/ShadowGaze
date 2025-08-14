using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Core.Services.Extensions;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.UpdatingMessages;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.CallbackQueries.Endpoints;

public class GetEndpointProcessor(
    PublicBotProperties botProperties,
    CustomersRepository customersRepository,
    EndpointsRepository endpointsRepository
) : BaseUpdateProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, _) =>
        type is UpdateTypes.CallbackQuery && update is { CallbackQuery.Data: CallbackQueriesConstants.Endpoints };

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

    private async Task EditMessageGetEndpoint(CallbackQuery query, int endpointId)
    {
        var builder = new InlineKeyboardBuilder();
        var chatId = query.Message!.Chat.Id;

        await Api.AnswerCallbackQueryAsync(new AnswerCallbackQueryArgs(query.Id));
        var endpoint = await endpointsRepository.GetByIdAsync(endpointId);
        using var ms = new MemoryStream(endpoint.QRCode, writable: false);
        var input = new InputFile(ms, "test");
        var endpointArgs = new SendPhotoArgs(chatId, input)
        {
            Caption = $"`{endpoint.ConnectionString}`",
            ParseMode = "MarkdownV2"
        };
        await Api.SendPhotoAsync(endpointArgs);
        var messageArgs = new SendMessageArgs(chatId, "Следуйте инструкциям по использованию")
        {
            ChatId = query.Message.Chat.Id,
            ReplyMarkup = GetKeyboard()
        };
        await Api.SendMessageAsync(messageArgs);
    }

    private InlineKeyboardMarkup GetKeyboard()
    {
        return BuildKeyboard()
            .AppendCallbackData("Инструкции", CallbackQueriesConstants.Instructions)
            .AppendRow()
            .AppendCallbackData("Главное меню", CallbackQueriesConstants.MainMenu)
            .Build();
    }

    private async Task EditMessageCreateEndpoint(CallbackQuery query)
    {
        await Api.EditMessageTextAsync(query.Message.Chat.Id, query.Message.MessageId,
            "Вам доступен пробный период: Ссылка для подключения появится в боте в течении часа");
        await Api.SendMessageAsync(-4929973929,
            $"Username: {query.Message.Chat.Username} ID: {query.Message.Chat.Id} хочет прокси");
    }
}