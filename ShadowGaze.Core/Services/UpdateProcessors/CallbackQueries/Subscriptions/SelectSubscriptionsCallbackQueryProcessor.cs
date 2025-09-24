using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        var chatId = query.Message!.Chat.Id;
        var currency = "RUB";
        var lp = new LabeledPrice("Продлить", subscriptionDescription.CentsAmount);
        var args = new SendInvoiceArgs(chatId,
            "Продление доступа",
            $"Период пользования будет увеличен на {subscriptionDescription.DurationInMonths} {subscriptionDescription.Estimate.ToLower()}",
            subscriptionDescription.DurationInMonths.ToString(),
            currency,
            [lp])
        {
            ProviderToken = providerToken,
            NeedEmail = true,
            SendEmailToProvider = true,
            ProviderData = GetProviderData(
                $"Продление периода пользования proxy на {subscriptionDescription.DurationInMonths} {subscriptionDescription.Estimate.ToLower()}",
                currency,
                subscriptionDescription.Amount)
        };
        await Bot.SendInvoiceAsync(args);
    }

    /// <summary>
    /// Данные для формирования чека
    /// </summary>
    /// <param name="description">Описание товара/услуги</param>
    /// <param name="currency">Трехбуквенный код валюты в формате ISO-4217</param>
    /// <param name="amount">Сумма в выбранной валюте. Всегда дробное значение</param>
    /// <returns></returns>
    private string GetProviderData(string description, string currency, double amount)
    {

        string formattedAmount = amount.ToString("F2", CultureInfo.InvariantCulture);
        var providerData = new
        {
            receipt = new
            {
                items = new[]
                {
                    new
                    {
                        description = description,
                        quantity = "1.00",
                        amount = new { value = formattedAmount, currency = currency },
                        vat_code = 1,
                        payment_mode = "full_payment",
                        payment_subject = "service"
                    }
                }
            }
        };

        // Сериализация в строку JSON (Telegram ожидает string)
        var jsonOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        return JsonSerializer.Serialize(providerData, jsonOptions);
    }
}