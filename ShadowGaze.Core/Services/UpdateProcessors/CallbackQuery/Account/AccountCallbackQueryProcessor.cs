using ShadowGaze.Core.Models;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.UpdatingMessages;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.CallbackQuery.Account;

public class AccountCallbackQueryProcessor(
    PublicBotProperties botProperties,
    CustomersRepository customersRepository) : BaseCallbackQueryProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, context) =>
        type is UpdateTypes.CallbackQuery && update.CallbackQuery?.Data == "main;get_account";
    public override async Task Process(Update update)
    {
        var query = update.CallbackQuery;
        await Api.AnswerCallbackQueryAsync(new AnswerCallbackQueryArgs(query.Id));
        
        var user = update.CallbackQuery?.From;
        if (user == null)
        {
            return;
        }
        var customer = await customersRepository.GetOrCreateAsync(user.Id, user.Username);
        
        var builder = new InlineKeyboardBuilder();
        builder.AppendCallbackData("Пополнить", $"account;top_up");
        await Api.EditMessageTextAsync<Message>(
            new EditMessageTextArgs($"Ваш баланс {customer.Balance}")
            {
                ChatId = query.Message.Chat.Id,
                MessageId = query.Message.MessageId,
                ReplyMarkup = new InlineKeyboardMarkup(builder)
            });
    }
}