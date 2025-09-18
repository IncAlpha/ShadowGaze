using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Core.Models.SessionContexts;
using ShadowGaze.Core.Models.SessionContexts.PromotionCodes;
using ShadowGaze.Data.Models.Database;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.UpdatingMessages;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.CallbackQueries.PromotionalCodes;

public class PromotionalCodeCallbackQueryProcessor(PublicBotProperties botProperties) : BaseUpdateProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, context) =>
        type is UpdateTypes.CallbackQuery && update.CallbackQuery?.Data == CallbackQueriesConstants.PromotionalCodes;
    public override async Task Process(Update update, SessionContext sessionContext)
    {
        var query = update.CallbackQuery!;
        await Bot.AnswerCallbackQueryAsync(new AnswerCallbackQueryArgs(query.Id));

        sessionContext.UpdateState(new PromotionCodeContext());
        await Bot.EditMessageTextAsync<Message>(
            new EditMessageTextArgs("Введите промокод")
            {
                ChatId = query.Message!.Chat.Id,
                MessageId = query.Message.MessageId,
                ReplyMarkup = GetKeyboard()
            });
    }
    
    private InlineKeyboardMarkup GetKeyboard()
    {
        return BuildKeyboard()
            .AppendCallbackData("Главное меню", CallbackQueriesConstants.MainMenu)
            .Build();
    }
}

public class PromotionalCodeMessageUpdateProcessor(
    PublicBotProperties botProperties, 
    PromotionalCodeRepository promotionalCodeRepository,
    PromotionalCodeUsageRepository promotionalCodeUsageRepository,
    CustomersRepository customersRepository)
    : BaseUpdateProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter  => (type, update, context) =>
        type is UpdateTypes.Message && update.Message?.Text is not null && context.State is PromotionCodeContext;
    public override async Task Process(Update update, SessionContext sessionContext)
    {
        var chatId = update.Message!.Chat.Id;
        var userId = update.Message!.From!.Id;
        var code = update.Message!.Text!.Trim();
        var codeEntity = await promotionalCodeRepository.GetByValueAsync(code);
        if (codeEntity is null)
        {
            var arg = new SendMessageArgs(chatId, "Промокод не найден. Попробуйте еще раз")
            {
                ReplyMarkup = GetKeyboard()
            };
            await Bot.SendMessageAsync(arg);
            return;
        }
        
        var customerEntity = await customersRepository.GetByTelegramIdAsync(userId);
        var usageEntity = await promotionalCodeUsageRepository.GetByPrimaryKey(customerEntity.Id, codeEntity.Id);
        if (usageEntity != null)
        {
            var arg = new SendMessageArgs(chatId, "Промокод уже применен")
            {
                ReplyMarkup = GetKeyboard()
            };
            await  Bot.SendMessageAsync(arg);
            return;  
        }
        
        if (codeEntity.StartDate >= DateTime.UtcNow || codeEntity.EndDate <= DateTime.UtcNow)
        {
            var cstZone = TimeZoneInfo.CreateCustomTimeZone("RuusianMoskow", TimeSpan.FromHours(3),
                "RuusianMoskow", "RuusianMoskow");
            var arg = new SendMessageArgs(chatId,
                $"Не удалось применить промокод.\n" +
                $"Время действия\n" +
                $"от {TimeZoneInfo.ConvertTimeFromUtc(codeEntity.StartDate, cstZone)}\n" +
                $"до {TimeZoneInfo.ConvertTimeFromUtc(codeEntity.EndDate, cstZone)}")
            {
                ReplyMarkup = GetKeyboard()
            };
            await Bot.SendMessageAsync(arg);
            return;  
        }

        var promotionalUsage = new PromotionalCodeUsage()
        {
            CustomerId = customerEntity.Id,
            PromotionalCodeId = codeEntity.Id,
        };
        await promotionalCodeUsageRepository.AddAsync(promotionalUsage);
        
        //todo change endpoint expiry date
        
        var applyPromotional = new SendMessageArgs(chatId, $"Промокод применен. Вам добавлено {codeEntity.Duration.TotalDays}")
        {
            ReplyMarkup = GetKeyboard()
        };
        await Bot.SendMessageAsync(applyPromotional);
    }
    
    private InlineKeyboardMarkup GetKeyboard()
    {
        return BuildKeyboard()
            .AppendCallbackData("Главное меню", CallbackQueriesConstants.MainMenu)
            .Build();
    }
}