using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.SessionContexts;
using ShadowGaze.Core.Models.SessionContexts.Instructions;
using ShadowGaze.Core.Services.Extensions;
using ShadowGaze.Data.Models.Database.TelegramFiles;
using ShadowGaze.Data.Services.Database.Instructions;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.UpdatingMessages;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.Messages.AdminCommands.AddPlatformInstruction;

public class SetFileProcessor(
    PublicBotProperties botProperties,
    PlatformInstructionsRepository instructionsRepository
) : BaseUpdateProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, context) =>
        type is UpdateTypes.Message &&
        context.State is PlatformInstructionContext
        {
            PlatformInstruction:
            {
                Platform: not null,
                Description: not null,
                TelegramFile: null,
                ApplicationName: null,
                ApplicationUrl: null
            }
        };

    public override async Task Process(Update update, SessionContext sessionContext)
    {
        var message = update.Message!;
        var chatId = message.Chat.Id;

        var context = sessionContext.State as PlatformInstructionContext;
        var instruction = context!.PlatformInstruction;
        var caption = message.Caption!;

        if (message.Video is null &&
            message.Animation is null ||
            message.Caption is null)
        {
            var errorArgs = new SendMessageArgs(chatId, "Отправь видео с инструкцией и подписью, " +
                                                        "в которой сначала указано название приложения, а затем с " +
                                                        "новой строки ссылка на приложение.");
            await Bot.SendMessageAsync(errorArgs);
            return;
        }

        var newFile = new TelegramFile
        {
            FileId = message.GetMediaFileId(),
            FileUniqueId = message.GetMediaFileUniqueId(),
            FileType = message.GetFileType(),
            Description = $"Инструкция для {instruction.Platform} - {instruction.ApplicationName}"
        };

        var applicationName = caption.Split("\n")[0].Trim();
        var applicationUrl = caption.Split("\n")[1].Trim();
        instruction.ApplicationName = applicationName;
        instruction.ApplicationUrl = applicationUrl;
        instruction.TelegramFile = newFile;
        await Bot.DeleteMessageAsync(chatId, message.MessageId);

        await instructionsRepository.SaveAsync(instruction);

        if (sessionContext.LastSentMessage is not null)
        {
            var editArgs = new EditMessageTextArgs("Готово! Вот превью инструкции:")
            {
                ChatId = chatId,
                MessageId = sessionContext.LastSentMessage
            };

            await Bot.EditMessageTextAsync<Message>(editArgs);

            await Bot.SendFileAsync(instruction.BuildSendMessageArgs(chatId));
        }
    }
}