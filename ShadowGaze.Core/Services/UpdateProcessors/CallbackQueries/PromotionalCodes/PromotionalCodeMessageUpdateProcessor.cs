using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Core.Models.SessionContexts;
using ShadowGaze.Core.Models.SessionContexts.PromotionCodes;
using ShadowGaze.Data.Models.Database;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.CallbackQueries.PromotionalCodes;

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
        
        var customerEntity = await customersRepository.GetByTelegramIdWithEndpointAsync(userId);
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
            var cstZone = TimeZoneInfo.CreateCustomTimeZone("RussianMoscow", TimeSpan.FromHours(3),
                "RussianMoscow", "RussianMoscow");
            var arg = new SendMessageArgs(chatId,
                $"Не удалось применить промокод.\n" +
                $"Время действия\n" +
                $"от {TimeZoneInfo.ConvertTimeFromUtc(codeEntity.StartDate, cstZone):yyyy-MM-dd HH:mm} МСК\n" +
                $"до {TimeZoneInfo.ConvertTimeFromUtc(codeEntity.EndDate, cstZone):yyyy-MM-dd HH:mm} МСК")
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
        
        
        if (customerEntity is { Endpoint: not null })
        {
            //TODO update expiry date in Xray
            customerEntity.Endpoint.ExpiryDate =
                customerEntity.Endpoint.ExpiryDate.AddDays(codeEntity.Duration.TotalDays);
            await customersRepository.SaveAsync(customerEntity);
        }
        
        var applyPromotional = new SendMessageArgs(chatId, $"Промокод применен. Вам добавлено {codeEntity.Duration.TotalDays} дней")
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