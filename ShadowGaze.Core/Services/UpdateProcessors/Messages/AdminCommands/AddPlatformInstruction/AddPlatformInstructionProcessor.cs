using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Core.Models.SessionContexts;
using ShadowGaze.Core.Models.SessionContexts.Instructions;
using ShadowGaze.Core.Services.UpdateProcessors.CallbackQueries.Instructions;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.Messages.AdminCommands.AddPlatformInstruction;

public class AddPlatformInstructionProcessor(
    PublicBotProperties botProperties
) : BaseUpdateProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, _) =>
        type is UpdateTypes.Message && update.Message?.Text == AdminCommandsConstants.AddInstruction;

    public override async Task Process(Update update, SessionContext sessionContext)
    {
        sessionContext.UpdateState(new PlatformInstructionContext());
        await AskPlatform(update);
    }

    private async Task AskPlatform(Update update)
    {
        var message = update.Message!;
        var chatId = message.Chat.Id;


        var sendArgs = new SendMessageArgs(chatId, "Выбери платформу для которой добавить инструкцию:")
        {
            ChatId = chatId,
            ReplyMarkup = GetKeyboard()
        };

        await Bot.SendMessageAsync(sendArgs);
    }

    private InlineKeyboardMarkup GetKeyboard()
    {
        var builder = BuildKeyboard();
        foreach (var (platform, button) in InstructionsProcessor.PlatformButtons)
        {
            builder
                .AppendRow()
                .AppendCallbackData(button,
                    $"{CallbackQueriesConstants.Instructions};set_platform;{platform}");
        }

        return builder
            .AppendRow()
            .AppendCallbackData("Отмена", CallbackQueriesConstants.Cancel)
            .Build();
    }
}