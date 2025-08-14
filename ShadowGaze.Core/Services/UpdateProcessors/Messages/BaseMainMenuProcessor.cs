using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.Messages;

public abstract class BaseMainMenuProcessor(PublicBotProperties botProperties) : BaseUpdateProcessor(botProperties)
{
    public abstract override Func<UpdateTypes, Update, SessionContext, bool> Filter { get; }

    public abstract override Task Process(Update update);

    protected virtual InlineKeyboardMarkup GetKeyboard()
    {
        return BuildKeyboard()
            .AppendCallbackData("Получить ссылку", CallbackQueriesConstants.Endpoints)
            .AppendCallbackData("Выбрать подписку", CallbackQueriesConstants.Subscriptions)
            .AppendRow()
            .AppendCallbackData("Мой баланс", CallbackQueriesConstants.Accounts)
            .AppendRow()
            .AppendCallbackData("Инструкции", CallbackQueriesConstants.Instructions)
            .Build();
    }
}