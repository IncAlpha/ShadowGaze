using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Core.Models.SessionContexts;
using ShadowGaze.Core.Models.SessionContexts.Instructions;
using ShadowGaze.Data.Models;
using ShadowGaze.Data.Models.Database.Instructions;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.UpdatingMessages;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.Messages.AdminCommands.AddPlatformInstruction;

public class SetPlatformProcessor(PublicBotProperties botProperties) : BaseUpdateProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, context) =>
        type is UpdateTypes.CallbackQuery &&
        (update.CallbackQuery?.Data?.StartsWith($"{CallbackQueriesConstants.Instructions};set_platform;") ?? false) &&
        context.State is PlatformInstructionContext
        {
            PlatformInstruction: null
        };

    public override async Task Process(Update update, SessionContext sessionContext)
    {
        var query = update.CallbackQuery!;
        var message = query.Message!;
        var chatId = message.Chat.Id;

        var platform = Enum.Parse<Platforms>(query.Data!.Split(";")[2]);
        var context = sessionContext.State as PlatformInstructionContext;
        var instruction = new PlatformInstruction
        {
            Platform = platform
        };
        context!.PlatformInstruction = instruction;

        var editArgs = new EditMessageTextArgs("Отправь описание инструкции")
        {
            ChatId = chatId,
            MessageId = message.MessageId
        };
        sessionContext.LastSentMessage = message.MessageId;

        await Bot.EditMessageTextAsync<Message>(editArgs);
    }
}