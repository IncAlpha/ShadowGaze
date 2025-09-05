using Microsoft.Extensions.Logging;
using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Core.Models.SessionContexts;
using ShadowGaze.Data.Services.Database.Instructions;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.CallbackQueries.Instructions;

public class GetInstructionByIdProcessor(
    ILogger<GetInstructionByIdProcessor> logger,
    PublicBotProperties botProperties,
    PlatformInstructionsRepository instructionsRepository
) : BaseInstructionProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, _) =>
        type is UpdateTypes.CallbackQuery &&
        (update.CallbackQuery?.Data?.StartsWith($"{CallbackQueriesConstants.Instructions};get_by_id;") ?? false);

    public override async Task Process(Update update, SessionContext sessionContext)
    {
        var query = update.CallbackQuery!;
        var message = query.Message!;
        var chatId = message.Chat.Id;

        if (!int.TryParse(query.Data!.Split(";")[2], out var instructionId))
        {
            logger.LogError($"Пришел неверный id инструкции. Query data: {query.Data}");
            return;
        }

        await Bot.AnswerCallbackQueryAsync(new AnswerCallbackQueryArgs(query.Id));

        var instruction = await instructionsRepository.GetForMessageAsync(instructionId);

        if (instruction is null)
        {
            logger.LogError($"Не удалось найти инструкцию с ID: {instructionId}");
            return;
        }
        
        await ProcessSingleInstruction(query, instruction);
    }
}