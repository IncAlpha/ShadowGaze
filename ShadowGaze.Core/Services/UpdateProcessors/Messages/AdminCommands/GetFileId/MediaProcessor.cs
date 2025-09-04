using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.SessionContexts;
using ShadowGaze.Core.Models.SessionContexts.GetFileId;
using ShadowGaze.Core.Services.Extensions;
using ShadowGaze.Data.Models.Database.TelegramFiles;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.Messages.AdminCommands.GetFileId;

public class MediaProcessor(
    PublicBotProperties botProperties,
    TelegramFilesRepository filesRepository
) : BaseUpdateProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, context) =>
        type is UpdateTypes.Message &&
        update.Message.HasMedia() &&
        context.State is FileContext;

    public override async Task Process(Update update, SessionContext sessionContext)
    {
        var message = update.Message!;
        var chatId = message.Chat.Id;

        var newFile = new TelegramFile
        {
            FileId = message.GetMediaFileId(),
            FileUniqueId = message.GetMediaFileUniqueId(),
            FileType = message.GetFileType(),
            Description = message.Caption
        };

        var sendArgs = new SendMessageArgs(chatId, $"Файл \\({newFile.Description.EscapeMarkdownCommon()}\\) сохранён:\n`{newFile.FileId.EscapeMarkdownSpecial()}`")
        {
            ChatId = chatId,
            ParseMode = "MarkdownV2"
        };

        await filesRepository.SaveAsync(newFile);
        sessionContext.UpdateState(null);

        await Bot.SendMessageAsync(sendArgs);
    }
}