using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.SessionContexts;
using ShadowGaze.Core.Models.SessionContexts.Instructions;
using ShadowGaze.Core.Services.Extensions;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.UpdatingMessages;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.Messages.AdminCommands.AddPlatformInstruction;

public class SetDescriptionProcessor(PublicBotProperties botProperties) : BaseUpdateProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, _, context) =>
        type is UpdateTypes.Message &&
        context.State is PlatformInstructionContext
        {
            PlatformInstruction:
            {
                Platform: not null,
                Description: null
            }
        };

    public override async Task Process(Update update, SessionContext sessionContext)
    {
        var message = update.Message!;
        var chatId = message.Chat.Id;

        var context = sessionContext.State as PlatformInstructionContext;
        var instruction = context!.PlatformInstruction;
        var description = message.GetMarkdownText();
        instruction.Description = description;
        await Bot.DeleteMessageAsync(chatId, message.MessageId);

        if (sessionContext.LastSentMessage is not null)
        {
            var text = "Отправь видео с инструкцией. В описании укажи сначала название " +
                       "приложения, затем с **новой строки** ссылку на приложение";
            var editArgs = new EditMessageTextArgs(text.EscapeMarkdownCommon())
            {
                ChatId = chatId,
                MessageId = sessionContext.LastSentMessage,
                ParseMode = "MarkdownV2"
            };

            await Bot.EditMessageTextAsync<Message>(editArgs);
        }
    }
}