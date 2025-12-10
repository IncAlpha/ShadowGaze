using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Core.Models.SessionContexts;
using ShadowGaze.Core.Models.SessionContexts.PromotionCodes;
using ShadowGaze.Core.Services.Extensions;
using ShadowGaze.Data.Models.Database;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.Messages.MainMenu;

public abstract class BaseMainMenuProcessor(
    PublicBotProperties botProperties,
    CustomersRepository customersRepository,
    BotSectionsRepository sectionsRepository
) : BaseUpdateProcessor(botProperties)
{
    public abstract override Func<UpdateTypes, Update, SessionContext, bool> Filter { get; }

    private Customer _customer;

    public override async Task Process(Update update, SessionContext sessionContext)
    {
        sessionContext.UpdateState(new EmptyContext());
        await InternalProcess(update, sessionContext);
        var userId = GetUserId(update);
        var chatId = GetChatId(update);

        if (update.CallbackQuery is not null)
        {
            await Bot.AnswerCallbackQueryAsync(new AnswerCallbackQueryArgs(update.CallbackQuery.Id));
        }

        _customer = await customersRepository.GetByTelegramIdAsync(userId);
        if (ExistUser())
        {
            await AnswerExistUser(chatId);
            return;
        }
        
        sessionContext.UpdateState(new IsNewUserContext());
        await AnswerNewUser(chatId);
    }

    protected abstract long GetUserId(Update update);
    protected abstract long GetChatId(Update update);

    protected virtual Task InternalProcess(Update update, SessionContext sessionContext)
    {
        return Task.CompletedTask;
    }

    private async Task AnswerExistUser(long chatId)
    {
        var endDate = _customer.ExpiryDate;
        var text = $"Остаток дней: {(endDate - DateTime.Now).Days}\nДата окончания: {endDate:dd-MMMM-yyyy}";

        var header = await sectionsRepository.GetByNameAsync("main_menu");

        // TODO: отрефакторить после своей реализации отправки/редактирования сообщений с файлами и без (единая структура Message)
        if (header is null)
        {
            var sendMessageArgs = new SendMessageArgs(chatId, text)
            {
                ReplyMarkup = BuildMainMenuKeyboard()
            };
            await Bot.SendMessageAsync(sendMessageArgs);
            return;
        }

        var file = header.TelegramFile;
        var messageArgs = new SendPhotoArgs(chatId, file.FileId)
        {
            Caption = text,
            ReplyMarkup = BuildMainMenuKeyboard()
        };
        await Bot.SendFileAsync(messageArgs);
    }

    private async Task AnswerNewUser(long chatId)
    {
        var text = "ShadowGaze from BigBroTeam";
        var header = await sectionsRepository.GetByNameAsync("main_menu;new_user");

        // TODO: отрефакторить после своей реализации отправки/редактирования сообщений с файлами и без (единая структура Message)
        if (header is null)
        {
            var sendMessageArgs = new SendMessageArgs(chatId, text)
            {
                ReplyMarkup = BuildNewUserKeyboard()
            };
            await Bot.SendMessageAsync(sendMessageArgs);
            return;
        }

        var file = header.TelegramFile;
        var messageArgs = new SendPhotoArgs(chatId, file.FileId)
        {
            Caption = text,
            ReplyMarkup = BuildNewUserKeyboard()
        };
        await Bot.SendFileAsync(messageArgs);
    }

    private bool ExistUser()
    {
        return _customer != null;
    }

    private InlineKeyboardMarkup BuildMainMenuKeyboard()
    {
        return BuildKeyboard()
            .AppendCallbackData("Proxy", CallbackQueriesConstants.Endpoints)
            .AppendCallbackData("Продлить", CallbackQueriesConstants.Subscriptions)
            .AppendRow()
            .AppendCallbackData("У меня есть промокод", CallbackQueriesConstants.PromotionalCodes)
            .AppendRow()
            .AppendUrl("Связаться с нами", "https://t.me/shadowgazeproxy")
            // .AppendCallbackData("Реферальная программа", CallbackQueriesConstants.Referrals)
            .Build();
    }

    private InlineKeyboardMarkup BuildNewUserKeyboard()
    {
        return BuildKeyboard()
            .AppendCallbackData("Proxy", CallbackQueriesConstants.Endpoints)
            .Build();
    }
}