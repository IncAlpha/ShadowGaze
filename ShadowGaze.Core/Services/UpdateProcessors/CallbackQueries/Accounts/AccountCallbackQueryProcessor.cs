using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Core.Models.SessionContexts;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.UpdatingMessages;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.CallbackQueries.Accounts;

public class AccountCallbackQueryProcessor(
    PublicBotProperties botProperties,
    CustomersRepository customersRepository
) : BaseUpdateProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, _) =>
        type is UpdateTypes.CallbackQuery && update.CallbackQuery?.Data == CallbackQueriesConstants.Accounts;

    public override async Task Process(Update update, SessionContext sessionContext)
    {
        var query = update.CallbackQuery!;
        await Bot.AnswerCallbackQueryAsync(new AnswerCallbackQueryArgs(query.Id));

        var user = update.CallbackQuery?.From;
        if (user == null)
        {
            return;
        }

        var customer = await customersRepository.GetOrCreateAsync(user.Id, user.Username);

        var keyboard = BuildKeyboard()
            .AppendCallbackData("Пополнить", $"{CallbackQueriesConstants.Accounts};topup")
            .AppendRow()
            .AppendCallbackData("Назад", CallbackQueriesConstants.MainMenu)
            .Build();
        await Bot.EditMessageTextAsync<Message>(
            new EditMessageTextArgs($"Ваш баланс {customer.Balance}")
            {
                ChatId = query.Message!.Chat.Id,
                MessageId = query.Message.MessageId,
                ReplyMarkup = keyboard
            });
    }
}