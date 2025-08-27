using Microsoft.Extensions.Logging;
using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Data.Models;
using ShadowGaze.Data.Models.Database.Instructions;
using ShadowGaze.Data.Services.Database.Instructions;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.UpdatingMessages;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.CallbackQueries.Instructions;

public class GetInstructionsProcessor(
    ILogger<GetInstructionsProcessor> logger,
    PublicBotProperties botProperties,
    PlatformInstructionsRepository platformInstructionsRepository
) : BaseUpdateProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, _) =>
        type is UpdateTypes.CallbackQuery &&
        (update.CallbackQuery?.Data?.StartsWith($"{CallbackQueriesConstants.Instructions};get") ?? false);

    public override async Task Process(Update update)
    {
        var query = update.CallbackQuery!;
        var message = query.Message!;
        var chatId = message.Chat.Id;

        if (!Enum.TryParse<Platforms>(query.Data!.Split(";")[2], out var platform))
        {
            logger.LogError($"Пришел неверный тип платформы. Query data: {query.Data}");
            return;
        }

        await Api.AnswerCallbackQueryAsync(new AnswerCallbackQueryArgs(query.Id));

        var instructions = await platformInstructionsRepository.GetForPlatformAsync(platform);

        switch (instructions.Length)
        {
            case 0:
                await ProcessNoneInstruction(query, platform);
                break;
            case 1:
                await ProcessSingleInstruction(query, instructions[0]);
                break;
            case >= 1:
                ProcessMultipleInstruction();
                return;
        }
    }

    private async Task ProcessNoneInstruction(CallbackQuery query, Platforms platform)
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
        await Api.EditMessageTextAsync<Message>(messageArgs);
    }

    private async Task ProcessSingleInstruction(CallbackQuery query, PlatformInstruction instruction)
    {
        var message = query.Message!;
        var chatId = message.Chat.Id;
        
        _ = Api.DeleteMessageAsync(chatId, message.MessageId);

        var videoMessageArgs = new SendVideoArgs(chatId, instruction.VideoPath);
        await Api.SendVideoAsync(videoMessageArgs);

        var keyboardBuilder = BuildKeyboard();
        keyboardBuilder
            .AppendRow()
            .AppendCallbackData("Назад", CallbackQueriesConstants.Instructions);
        var messageText =
            $"""
            📲 *Подключение к сервису*  

            🔹 *Имя приложения:*  
            `{instruction.ApplicationName}`  

            🔹 *Ссылка на скачивание:*  
            [Скачать приложение]({instruction.ApplicationUrl})  

            🔹 *Описание:*  
            {instruction.Description}  
            """;
        var messageArgs = new SendMessageArgs(chatId, messageText)
        {
            ReplyMarkup = keyboardBuilder.Build(),
            ParseMode = "MarkdownV2"
        };
        await Api.SendMessageAsync(messageArgs);
    }

    private void ProcessMultipleInstruction()
    {
        // TODO: добавить обработку нескольких инструкций для одной платформы, добавить к сообщению кнопки с названиями приложений
    }
}