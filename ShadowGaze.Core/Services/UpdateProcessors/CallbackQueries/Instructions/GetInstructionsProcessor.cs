using Microsoft.Extensions.Logging;
using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Core.Models.SessionContexts;
using ShadowGaze.Data.Models;
using ShadowGaze.Data.Services.Database.Instructions;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.CallbackQueries.Instructions;

public class GetInstructionsProcessor(
    ILogger<GetInstructionsProcessor> logger,
    PublicBotProperties botProperties,
    PlatformInstructionsRepository platformInstructionsRepository
) : BaseInstructionProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, _) =>
        type is UpdateTypes.CallbackQuery &&
        (update.CallbackQuery?.Data?.StartsWith($"{CallbackQueriesConstants.Instructions};get;") ?? false);

    public override async Task Process(Update update, SessionContext sessionContext)
    {
        var query = update.CallbackQuery!;
        var message = query.Message!;
        var chatId = message.Chat.Id;

        if (!Enum.TryParse<Platforms>(query.Data!.Split(";")[2], out var platform))
        {
            logger.LogError($"Пришел неверный тип платформы. Query data: {query.Data}");
            return;
        }

        await Bot.AnswerCallbackQueryAsync(new AnswerCallbackQueryArgs(query.Id));

        var instructions = await platformInstructionsRepository.GetByPlatformAsync(platform);

        switch (instructions.Length)
        {
            case 0:
                await ProcessNoneInstruction(query, platform);
                break;
            case 1:
                var instructionId = instructions[0].Id;
                var instruction = await platformInstructionsRepository.GetForMessageAsync(instructionId);
                await ProcessSingleInstruction(query, instruction);
                break;
            case >= 1:
                await ProcessMultipleInstruction(query, instructions);
                return;
        }
    }
}