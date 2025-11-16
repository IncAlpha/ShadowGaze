using Microsoft.EntityFrameworkCore;
using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Core.Models.SessionContexts;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.CallbackQueries.Connections;

public class ConnectionsListProcessor(
    PublicBotProperties botProperties,
    InboundButtonRepository inboundButtonsRepository
) : BaseUpdateProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, _) =>
        type is UpdateTypes.CallbackQuery && update is { CallbackQuery.Data: CallbackQueriesConstants.ConnectionsList };
    public override async Task Process(Update update, SessionContext sessionContext)
    {
        var query = update.CallbackQuery!;
        var message = query.Message!;
        var chatId = message.Chat.Id;
        await Bot.AnswerCallbackQueryAsync(new AnswerCallbackQueryArgs(query.Id));
        
        var buttons = await inboundButtonsRepository.AsQueryable().ToListAsync();
        var builder = BuildKeyboard();
        foreach (var button in buttons)
        {
            builder
                .AppendCallbackData(button.ButtonName, $"{CallbackQueriesConstants.ConnectionForInbound}_{button.InboundId}")
                .AppendRow();   
        }
        var messageArgs = new SendMessageArgs(chatId, "Выберите желаемую страну подключения")
        {
            ChatId = chatId,
            ReplyMarkup = builder.Build()
        };
        await Bot.SendMessageAsync(messageArgs);
    }
}