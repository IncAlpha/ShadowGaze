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

public class SetServerNameProcessor(
    PublicBotProperties botProperties,
    InboundConfigurationRepository inboundConfigurationRepository
) : BaseUpdateProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, _, context) =>
        type is UpdateTypes.Message &&
        context.State is ConnectionConfigurationContext
        {
            WaitingForSni: true,
            SelectedConfigurationId: not null
        };

    public override async Task Process(Update update, SessionContext sessionContext)
    {
        var message = update.Message!;
        var chatId = message.Chat.Id;
        var context = (ConnectionConfigurationContext)sessionContext.State;
        var configurationId = context.SelectedConfigurationId!.Value;
        var serverName = message.Text?.Trim();

        if (String.IsNullOrWhiteSpace(serverName))
        {
            await Bot.SendMessageAsync(new SendMessageArgs(chatId, "SNI не может быть пустым, отправь корректное значение"));
            return;
        }

        var configuration = await inboundConfigurationRepository.GetByIdAsync(configurationId);
        if (configuration is null)
        {
            await Bot.SendMessageAsync(new SendMessageArgs(chatId, "Конфигурация не найдена"));
            return;
        }

        configuration.ServerName = serverName;
        await inboundConfigurationRepository.SaveAsync(configuration);
        context.WaitingForSni = false;

        var sendArgs = new SendMessageArgs(chatId, $"SNI обновлен для {configuration.ConnectionName}: {configuration.ServerName}")
        {
            ReplyMarkup = BuildActionsKeyboard(configuration.Id)
        };

        await Bot.SendMessageAsync(sendArgs);
    }

    private InlineKeyboardMarkup BuildActionsKeyboard(int configurationId)
    {
        return BuildKeyboard()
            .AppendCallbackData("Заменить SNI", $"{CallbackQueriesConstants.ConnectionConfigurations};replace_sni;{configurationId}")
            .AppendRow()
            .AppendCallbackData("Очистить подключения", $"{CallbackQueriesConstants.ConnectionConfigurations};clear_connections;{configurationId}")
            .Build();
    }
}
