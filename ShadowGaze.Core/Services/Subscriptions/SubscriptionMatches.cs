namespace ShadowGaze.Core.Services.Subscriptions;

public class SubscriptionMatches
{
    private readonly Dictionary<int, SubscriptionDescription> _subscriptions = new()
    {
        {
            1, 
            new SubscriptionDescription
            {
                Estimate = "Месяц",
                DurationInMonths = 1, 
                Amount = 200, 
                CentsAmount = 20000
            }
        },
        {
            3,
            new SubscriptionDescription
            {
                Estimate = "Месяца",
                DurationInMonths = 3,
                Amount = 550,
                CentsAmount = 55000
            }
        },
        {
            6,
            new SubscriptionDescription
            {
                Estimate = "Месяцев",
                DurationInMonths = 6,
                Amount = 1000,
                CentsAmount = 100000
            }
        },
        {
            12,
            new SubscriptionDescription
            {
                Estimate = "Месяцев",
                DurationInMonths = 12,
                Amount = 1900,
                CentsAmount = 190000
            }
        }
    };

    public IEnumerable<SubscriptionDescription> GetAllSubscriptionDescription()
    {
        return _subscriptions.Values.AsEnumerable();
    }

    public SubscriptionDescription GetSubscriptionDescription(int id)
    {
        return _subscriptions[id];
    }
}