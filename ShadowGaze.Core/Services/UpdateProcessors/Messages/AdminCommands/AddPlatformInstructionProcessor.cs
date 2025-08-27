using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.Messages.AdminCommands;

public class AddPlatformInstructionProcessor(PublicBotProperties botProperties) : BaseUpdateProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, _) =>
        type is UpdateTypes.Message && update.Message?.Text == AdminCommandsConstants.AddInstruction;
    public override Task Process(Update update)
    {
        throw new NotImplementedException();
        // TODO: добавить здесь загрузку текста (ParseMode: MarkdownV2 стоит у инструкций) и видео ну и остальных полей
        // видео мы планировали запоминать как ID файла. Удаление и редактирование пока оставь на ручками в БД
    }
}