using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.UpdatingMessages;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.CallbackQueries.Subscriptions;

public class SubscriptionsCallbackQueryProcessor(PublicBotProperties botProperties)
    : BaseUpdateProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, context) =>
        type is UpdateTypes.CallbackQuery && update.CallbackQuery?.Data == CallbackQueriesConstants.Subscriptions;

    public override async Task Process(Update update)
    {
        var query = update.CallbackQuery;
        await Api.AnswerCallbackQueryAsync(new AnswerCallbackQueryArgs(query.Id));

        var keyboard = BuildKeyboard()
            // TODO: переделать на id
            .AppendCallbackData("1 Месяц - 200₽", $"{CallbackQueriesConstants.Subscriptions};get;1")
            .AppendCallbackData("3 Месяца - 550₽", $"{CallbackQueriesConstants.Subscriptions};get;3")
            .AppendRow()
            .AppendCallbackData("6 Месяцев - 1000₽", $"{CallbackQueriesConstants.Subscriptions};get;6")
            .AppendCallbackData("12 Месяцев - 1900₽", $"{CallbackQueriesConstants.Subscriptions};get;12")
            .AppendRow()
            .AppendCallbackData("Назад", CallbackQueriesConstants.MainMenu)
            .Build();
        await Api.EditMessageTextAsync<Message>(
            new EditMessageTextArgs("Выберите длительность подписки")
            {
                ChatId = query.Message.Chat.Id,
                MessageId = query.Message.MessageId,
                ReplyMarkup = keyboard
            });
    }
}