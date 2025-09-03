using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.SessionContexts;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.Payments;

public class CheckoutMessageProcessor (PublicBotProperties botProperties) : BaseUpdateProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, context) =>
        type is UpdateTypes.Message && update.Message is {SuccessfulPayment: not null};
    public override async Task Process(Update update, SessionContext sessionContext)
    {
        var sp = update.Message!.SuccessfulPayment!;
        // TODO save in database
        Console.WriteLine($"Currency: {sp.Currency}");
        Console.WriteLine($"InvoicePayload: {sp.InvoicePayload}");
        Console.WriteLine($"IsFirstRecurring: {sp.IsFirstRecurring}");
        Console.WriteLine($"IsRecurring: {sp.IsRecurring}");
        Console.WriteLine($"OrderInfo: {sp.OrderInfo}");
        Console.WriteLine($"ProviderPaymentChargeId: {sp.ProviderPaymentChargeId}");
        Console.WriteLine($"ShippingOptionId: {sp.ShippingOptionId}");
        Console.WriteLine($"SubscriptionExpirationDate: {sp.SubscriptionExpirationDate}");
        Console.WriteLine($"TelegramPaymentChargeId: {sp.TelegramPaymentChargeId}");
        Console.WriteLine($"TotalAmount: {sp.TotalAmount}");
    }
}