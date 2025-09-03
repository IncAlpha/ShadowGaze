using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Core.Models.SessionContexts;
using ShadowGaze.Data.Models.Database;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.Messages.MainMenu;

public abstract class BaseMainMenuProcessor(PublicBotProperties botProperties, CustomersRepository customersRepository) : BaseUpdateProcessor(botProperties)
{
    public abstract override Func<UpdateTypes, Update, SessionContext, bool> Filter { get; }
    
    private Customer _customer;
    public override async Task Process(Update update, SessionContext sessionContext)
    {
        var userId = GetUserId(update);
        _customer = await customersRepository.GetByTelegramIdWithEndpointAsync(userId);
        await AnswerProcess(update, sessionContext);
    }
    
    protected abstract long GetUserId(Update update);
    protected abstract Task AnswerProcess(Update update, SessionContext sessionContext);
    
    protected string GetAnswerText()
    {
        if (ExistUser())
        {
            return ExistUserAnswer();
        }
        return NewUserAnswer();
    }

    private string ExistUserAnswer()
    {
        var endDate = _customer.Endpoint.ExpiryDate;
        return $"Остаток дней: {(endDate - DateTime.Now).Days}\nДата окончания: {endDate:dd-MMMM-yyyy}";
    }
    
    private string NewUserAnswer()
    {
        return "ShadowGaze from BigBroTeam";    
    }

    private InlineKeyboardMarkup ExistUserKeyboard()
    {
        return BuildKeyboard()
            .AppendCallbackData("Proxy", CallbackQueriesConstants.Endpoints)
            .AppendCallbackData("Продлить", CallbackQueriesConstants.Subscriptions)
            .AppendRow()
            .AppendCallbackData("Связаться с нами", CallbackQueriesConstants.AboutAs)
            .AppendCallbackData("Реферальная программа", CallbackQueriesConstants.Referrals)
            .Build();   
    }

    protected InlineKeyboardMarkup GetKeyboard()
    {
        if (ExistUser())
        {
            return ExistUserKeyboard();
        }
        return NewUserKeyboard();
    }

    private InlineKeyboardMarkup NewUserKeyboard()
    {
        return BuildKeyboard()
            .AppendCallbackData("Proxy", CallbackQueriesConstants.Endpoints)
            .Build();
    }

    private bool ExistUser()
    {
        return !(_customer == null || _customer.Endpoint == null);
    }
}