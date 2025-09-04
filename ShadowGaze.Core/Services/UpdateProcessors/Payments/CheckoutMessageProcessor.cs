using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.SessionContexts;
using ShadowGaze.Core.Services.UpdateProcessors.Messages.MainMenu;
using ShadowGaze.Core.Services.XUI;
using ShadowGaze.Data.Models.Database;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.Payments;

public class CheckoutMessageProcessor(
    PublicBotProperties botProperties,
    CustomersRepository customersRepository,
    BotSectionsRepository botSectionsRepository,
    PaymentsRepository paymentsRepository) 
    : BaseMainMenuProcessor(botProperties, customersRepository, botSectionsRepository)
{
    private readonly CustomersRepository _customersRepository = customersRepository;

    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, context) =>
        type is UpdateTypes.Message && update.Message is {SuccessfulPayment: not null};
    protected override async Task InternalProcess(Update update, SessionContext sessionContext)
    {
        var successfulPayment = update.Message!.SuccessfulPayment!;
        var userTelegramId = update.Message.From!.Id;
        var customer = await _customersRepository.GetByTelegramIdWithEndpointAsync(userTelegramId);
        
        await paymentsRepository.SaveAsync(new Payment()
        {
            CustomerId = customer?.Id,
            PaymentDate = DateTime.Now,
            Currency = successfulPayment.Currency,
            TotalAmount = successfulPayment.TotalAmount,
            InvoicePayload = successfulPayment.InvoicePayload,
            ProviderPaymentChargeId = successfulPayment.ProviderPaymentChargeId,
            TelegramPaymentChargeId = successfulPayment.TelegramPaymentChargeId,
        });
        
        if (int.TryParse(successfulPayment.InvoicePayload, out var monthNumber) && customer is { Endpoint: not null })
        {
            //TODO update expiry date in Xray
            customer.Endpoint.ExpiryDate = customer.Endpoint.ExpiryDate.AddMonths(monthNumber);
            await _customersRepository.SaveAsync(customer);
        }
        
    }

    protected override long GetUserId(Update update)
    {
        return update.Message!.From!.Id;
    }

    protected override long GetChatId(Update update)
    {
        return update.Message!.Chat!.Id;
    }
}