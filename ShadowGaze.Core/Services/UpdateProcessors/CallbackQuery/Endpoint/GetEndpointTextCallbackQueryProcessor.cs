using ShadowGaze.Core.Models;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.CallbackQuery.Endpoint;

public class GetEndpointTextCallbackQueryProcessor(
    PublicBotProperties botProperties,
    EndpointsRepository endpointsRepository) : BaseCallbackQueryProcessor(botProperties)
{

    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, context) =>
        type is UpdateTypes.CallbackQuery && (update.CallbackQuery?.Data?.StartsWith("get_endpoint;text") ?? false);
    public override async Task Process(Update update)
    {
        await Api.AnswerCallbackQueryAsync(new AnswerCallbackQueryArgs(update.CallbackQuery.Id));
        var endpointID = int.Parse(update.CallbackQuery.Data.Split(";")[2]);
        var endpoint = endpointsRepository.GetById(endpointID);
        await Api.SendMessageAsync(update.CallbackQuery.Message.Chat.Id, endpoint.ConnectionString);
    }
}