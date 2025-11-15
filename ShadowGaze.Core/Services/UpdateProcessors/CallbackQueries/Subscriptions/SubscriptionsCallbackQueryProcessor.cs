using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Core.Models.SessionContexts;
using ShadowGaze.Core.Services.Subscriptions;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.UpdatingMessages;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.CallbackQueries.Subscriptions;

public class SubscriptionsCallbackQueryProcessor(PublicBotProperties botProperties, SubscriptionMatches subscriptionMatches)
    : BaseUpdateProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, context) =>
        type is UpdateTypes.CallbackQuery && update.CallbackQuery?.Data == CallbackQueriesConstants.Subscriptions;

    public override async Task Process(Update update, SessionContext sessionContext)
    {
        var query = update.CallbackQuery!;
        await Bot.AnswerCallbackQueryAsync(new AnswerCallbackQueryArgs(query.Id));
        
        var buttonInRow = 0;
        var keyboardBuilder = BuildKeyboard();
        foreach (var subscriptionDescription in subscriptionMatches.GetAllSubscriptionDescription())
        {
            if (buttonInRow == 2)
            {
                buttonInRow = 0;
                keyboardBuilder.AppendRow();
            }
            var buttonText = string.Format("{0} {1} - {2}₽", subscriptionDescription.DurationInMonths,
                subscriptionDescription.Estimate, subscriptionDescription.Amount);
            keyboardBuilder.AppendCallbackData(buttonText,
                $"{CallbackQueriesConstants.Subscriptions};get;{subscriptionDescription.DurationInMonths}");
            buttonInRow++;
        }
        
        var keyboard = keyboardBuilder
            .AppendRow()
            .AppendCallbackData("Назад", CallbackQueriesConstants.MainMenu)
            .Build();
        await Bot.EditMessageTextAsync<Message>(
            new EditMessageTextArgs("Выберите длительность подписки")
            {
                ChatId = query.Message!.Chat.Id,
                MessageId = query.Message.MessageId,
                ReplyMarkup = keyboard
            });
    }
}