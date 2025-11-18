using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Congifurations;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Core.Models.SessionContexts;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.CallbackQueries.Connections;

public class MainConnectionProcessor(
    PublicBotProperties botProperties,
    ILogger<MainConnectionProcessor> logger,
    CustomersRepository customersRepository,
    InboundConfigurationRepository inboundConfigurationRepository,
    ConnectionsRepository connectionsRepository,
    IOptions<ConnectionsOptions> connectionsOptions
) : SendConnectionProcessor(connectionsRepository, botProperties)
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
        
        var mainInbound = await inboundConfigurationRepository.GetByNameAsync(connectionsOptions.Value.MainConnectionName);
        if (mainInbound == null)
        {
            logger.LogError($"Not found main inbound with name {connectionsOptions.Value.MainConnectionName}");
            return;
        }
        await SendMessage(chatId, mainInbound, customer);
    }
}