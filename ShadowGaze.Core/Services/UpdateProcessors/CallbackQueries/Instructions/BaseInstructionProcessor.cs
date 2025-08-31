using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Core.Services.Extensions;
using ShadowGaze.Data.Models;
using ShadowGaze.Data.Models.Database.Instructions;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.UpdatingMessages;

namespace ShadowGaze.Core.Services.UpdateProcessors.CallbackQueries.Instructions;

public abstract class BaseInstructionProcessor(PublicBotProperties botProperties) : BaseUpdateProcessor(botProperties)
{
    protected async Task ProcessNoneInstruction(CallbackQuery query, Platforms platform)
    {
        var message = query.Message!;
        var chatId = message.Chat.Id;
        var keyboardBuilder = BuildKeyboard();
        keyboardBuilder
            .AppendRow()
            .AppendCallbackData("Назад", CallbackQueriesConstants.Instructions);

        var messageArgs = new EditMessageTextArgs($"Инструкции для платформы " +
                                                  $"{InstructionsProcessor.PlatformButtons[platform]} " +
                                                  $"скоро будут доступны")
        {
            ChatId = chatId,
            MessageId = message.MessageId,
            ReplyMarkup = keyboardBuilder.Build(),
            ParseMode = "MarkdownV2"
        };
        await Bot.EditMessageTextAsync<Message>(messageArgs);
    }

    protected async Task ProcessSingleInstruction(CallbackQuery query, PlatformInstruction instruction)
    {
        var message = query.Message!;
        var chatId = message.Chat.Id;

        await Bot.EditMessageMediaAsync<Message>(instruction.BuildEditMessageArgs(chatId, message.MessageId, builder =>
            builder
                .AppendRow()
                .AppendCallbackData("Назад", CallbackQueriesConstants.Instructions)));
    }

    protected async Task ProcessMultipleInstruction(CallbackQuery query, PlatformInstruction[] instructions)
    {
        var message = query.Message!;
        var chatId = message.Chat.Id;
        var keyboardBuilder = BuildKeyboard();
        foreach (var instruction in instructions)
        {
            keyboardBuilder
                .AppendRow()
                .AppendCallbackData(instruction.ApplicationName, $"{CallbackQueriesConstants.Instructions};get_by_id;{instruction.Id}");
        }

        keyboardBuilder
            .AppendRow()
            .AppendCallbackData("Назад", CallbackQueriesConstants.Instructions);

        var messageArgs = new EditMessageTextArgs("Выберите приложение:")
        {
            ChatId = chatId,
            MessageId = message.MessageId,
            ReplyMarkup = keyboardBuilder.Build(),
            ParseMode = "MarkdownV2"
        };
        await Bot.EditMessageTextAsync<Message>(messageArgs);
    }
}