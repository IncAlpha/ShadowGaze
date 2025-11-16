using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Core.Models.SessionContexts;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.CallbackQueries.Connections;

public class ConnectionForInboundProcessor(
    PublicBotProperties botProperties,
    CustomersRepository customersRepository,
    ConnectionsRepository connectionsRepository,
    InboundRepository inboundRepository) : SendConnectionProcessor(connectionsRepository, botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, _) =>
        type is UpdateTypes.CallbackQuery &&
        update.CallbackQuery is { Data: not null } &&
        update.CallbackQuery.Data.StartsWith(CallbackQueriesConstants.ConnectionForInbound);

    public override async Task Process(Update update, SessionContext sessionContext)
    {
        var inboundId = int.Parse(update.CallbackQuery!.Data!.Split("_")[1]);
        
        var query = update.CallbackQuery!;
        var message = query.Message!;
        var chatId = message.Chat.Id;
        var user = query.From;
        await Bot.AnswerCallbackQueryAsync(new AnswerCallbackQueryArgs(query.Id));
        
        var username = string.IsNullOrWhiteSpace(user.Username) ? chatId.ToString() : user.Username;
        var customer = await customersRepository.GetOrCreateAsync(user.Id, username);
        var inbound = await inboundRepository.GetByIdAsync(inboundId);
        
        await SendMessage(chatId, inbound, customer);
    }
}