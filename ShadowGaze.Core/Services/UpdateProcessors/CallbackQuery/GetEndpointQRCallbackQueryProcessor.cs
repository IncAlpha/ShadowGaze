using ShadowGaze.Core.Models;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.CallbackQuery;

public class GetEndpointQrCallbackQueryProcessor(
    PublicBotProperties botProperties,
    EndpointsRepository endpointsRepository) : BaseCallbackQueryProcessor(botProperties)
{

    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, context) =>
        type is UpdateTypes.CallbackQuery && update.CallbackQuery.Data.StartsWith("get_endpoint;text");
    public override async Task Process(Update update)
    {
        await Api.AnswerCallbackQueryAsync(new AnswerCallbackQueryArgs(update.CallbackQuery.Id));
        var endpointID = int.Parse(update.CallbackQuery.Data.Split(";")[2]);
        var endpoint = await endpointsRepository.GetByIdAsync(endpointID);
        using var ms = new MemoryStream(endpoint.QRCode, writable: false);
        var input = new InputFile(ms, "test");
        await Api.SendPhotoAsync(update.CallbackQuery.Message.Chat.Id, input);
    }
}