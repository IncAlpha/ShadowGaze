using ShadowGaze.Core.Models;
using ShadowGaze.Core.Models.Constants;
using ShadowGaze.Core.Models.SessionContexts;
using ShadowGaze.Core.Models.SessionContexts.ConnectionConfigurations;
using ShadowGaze.Data.Services.Database;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.UpdatingMessages;
using UpdateTypes = ShadowGaze.Data.Models.TelegramApi.UpdateTypes;

namespace ShadowGaze.Core.Services.UpdateProcessors.Messages.AdminCommands.ConnectionConfigurations;

public class SelectConnectionConfigurationProcessor(
    PublicBotProperties botProperties,
    InboundConfigurationRepository inboundConfigurationRepository
) : BaseUpdateProcessor(botProperties)
{
    public override Func<UpdateTypes, Update, SessionContext, bool> Filter => (type, update, context) =>
        type is UpdateTypes.CallbackQuery &&
        (update.CallbackQuery?.Data?.StartsWith($"{CallbackQueriesConstants.ConnectionConfigurations};select;") ?? false) &&
        context.State is ConnectionConfigurationContext;

    public override async Task Process(Update update, SessionContext sessionContext)
    {
        var callbackQuery = update.CallbackQuery!;
        var message = callbackQuery.Message!;
        var chatId = message.Chat.Id;
        var context = (ConnectionConfigurationContext)sessionContext.State;

        await Bot.AnswerCallbackQueryAsync(new AnswerCallbackQueryArgs(callbackQuery.Id));

        var configurationId = GetConfigurationId(callbackQuery.Data!);
        if (configurationId is null)
        {
            return;
        }

        var configuration = await inboundConfigurationRepository.GetByIdAsync(configurationId.Value);
        if (configuration is null)
        {
            return;
        }

        context.SelectedConfigurationId = configuration.Id;
        context.WaitingForSni = false;

        var text = $"Конфигурация: {configuration.ConnectionName}\nТекущий SNI: {configuration.ServerName}";

        var editArgs = new EditMessageTextArgs(text)
        {
            ChatId = chatId,
            MessageId = message.MessageId,
            ReplyMarkup = BuildActionsKeyboard(configuration.Id)
        };

        await Bot.EditMessageTextAsync<Message>(editArgs);
    }

    private InlineKeyboardMarkup BuildActionsKeyboard(int configurationId)
    {
        return BuildKeyboard()
            .AppendCallbackData("Заменить SNI", $"{CallbackQueriesConstants.ConnectionConfigurations};replace_sni;{configurationId}")
            .AppendRow()
            .AppendCallbackData("Очистить подключения", $"{CallbackQueriesConstants.ConnectionConfigurations};clear_connections;{configurationId}")
            .Build();
    }

    private int? GetConfigurationId(string data)
    {
        var splitData = data.Split(";");
        if (splitData.Length < 3)
        {
            return null;
        }

        return int.TryParse(splitData[2], out var configurationId)
            ? configurationId
            : null;
    }
}
