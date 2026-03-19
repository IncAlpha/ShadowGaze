using Microsoft.EntityFrameworkCore;
using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Core.Models.SessionContexts;
using ShadowGaze.Core.Models.SessionContexts.ConnectionConfigurations;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.Messages.AdminCommands.ConnectionConfigurations;

public class ConnectionConfigurationsCommandProcessor(
    PublicBotProperties botProperties,
    InboundConfigurationRepository inboundConfigurationRepository
) : BaseUpdateProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, _) =>
        type is UpdateTypes.Message && update.Message?.Text == AdminCommandsConstants.ConnectionConfiguration;

    public override async Task Process(Update update, SessionContext sessionContext)
    {
        var message = update.Message!;
        var chatId = message.Chat.Id;

        sessionContext.UpdateState(new ConnectionConfigurationContext());

        var configurations = await inboundConfigurationRepository.AsQueryable()
            .OrderBy(model => model.ConnectionName)
            .ToListAsync();

        if (configurations.Count == 0)
        {
            await Bot.SendMessageAsync(new SendMessageArgs(chatId, "Конфигурации не найдены"));
            return;
        }

        var keyboard = BuildKeyboard();
        foreach (var configuration in configurations)
        {
            keyboard.AppendCallbackData(
                configuration.ConnectionName,
                $"{CallbackQueriesConstants.ConnectionConfigurations};select;{configuration.Id}");
            keyboard.AppendRow();
        }

        var sendArgs = new SendMessageArgs(chatId, "Выбери конфигурацию для управления")
        {
            ReplyMarkup = keyboard.Build()
        };

        await Bot.SendMessageAsync(sendArgs);
    }
}
