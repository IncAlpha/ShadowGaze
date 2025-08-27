using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Data.Models.Database;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.Messages;

public abstract class BaseMainMenuProcessor(PublicBotProperties botProperties, CustomersRepository customersRepository) : BaseUpdateProcessor(botProperties)
{
    public abstract override Func<UpdateTypes, Update, SessionContext, bool> Filter { get; }
    
    private Customer _customer;
    public override async Task Process(Update update)
    {
        var userId = GetUserId(update);
        _customer = await customersRepository.GetByTelegramIdWithEndpointAsync(userId);
        await AnswerProcess(update);
    }
    
    protected abstract long GetUserId(Update update);
    protected abstract Task AnswerProcess(Update update);
    
    protected string GetAnswerText()
    {
        if (_customer == null || _customer.Endpoint == null)
        {
            return NewUserAnswer();
        }
        return ExistUserAnswer();    
    }
    
    private string NewUserAnswer()
    {
        return "ShadowGaze from BigBroTeam";    
    }

    private string ExistUserAnswer()
    {
        _customer.Endpoint.ExpiryDate = DateTime.Now.Date.AddDays(50); //TODO load from database
        var plan = "3 мес"; //TODO load from database
        var endDate = _customer.Endpoint.ExpiryDate;
        return $"Ваш план: {plan}\nОстаток дней: {(endDate - DateTime.Now).Days}\nДата окончания: {endDate:dd-MMMM-yyyy}";
    }

    protected InlineKeyboardMarkup GetKeyboard()
    {
        if (_customer == null)
        {
            return NewUserKeyboard();
        }
        return BuildKeyboard()
            .AppendCallbackData("Proxy", CallbackQueriesConstants.Endpoints)
            .AppendCallbackData("Обновить план", CallbackQueriesConstants.Subscriptions)
            .AppendRow()
            .AppendCallbackData("Связаться с нами", CallbackQueriesConstants.AboutAs)
            .AppendCallbackData("Реферальная программа", CallbackQueriesConstants.Referrals)
            .Build();
    }

    private InlineKeyboardMarkup NewUserKeyboard()
    {
        return BuildKeyboard()
            .AppendCallbackData("Proxy", CallbackQueriesConstants.Endpoints)
            .Build();
    }
}