using Microsoft.Extensions.Configuration;
using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Core.Models.SessionContexts;
using ShadowGaze.Core.Services.Subscriptions;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.Payments;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.CallbackQueries.Subscriptions;

public class SelectSubscriptionsCallbackQueryProcessor(
    PublicBotProperties botProperties, 
    SubscriptionMatches subscriptionMatches, 
    IConfiguration configuration)
    : BaseUpdateProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, _) =>
        type is UpdateTypes.CallbackQuery &&
        (update?.CallbackQuery?.Data?.StartsWith($"{CallbackQueriesConstants.Subscriptions};get;") ?? false);

    public override async Task Process(Update update, SessionContext sessionContext)
    {
        var query = update.CallbackQuery!;
        await Bot.AnswerCallbackQueryAsync(new AnswerCallbackQueryArgs(query.Id));
        var subType = int.Parse(query.Data!.Split(";")[2]);
        var subscriptionDescription = subscriptionMatches.GetSubscriptionDescription(subType);
        var secret = configuration.GetSection("secret");
        var providerToken = secret["providerToken"];
        // TODO Fill payment details
        var lp = new LabeledPrice("Оплата подписки", subscriptionDescription.CentsAmount);
        var args = new SendInvoiceArgs(query.Message!.Chat.Id,
            "SG Subscription",
            "SG Subscription",
            "This is payload",
            "RUB",
            [lp])
        {
            ProviderToken = providerToken
        };
        await Bot.SendInvoiceAsync(args);
    }
}