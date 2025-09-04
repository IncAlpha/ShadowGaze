using Microsoft.Extensions.Configuration;
using ShadowGaze.Core.Models.Constants;
using Telegram.BotAPI.GettingUpdates;

namespace ShadowGaze.Core.Services.Middlewares;

public class AdminCommandsMiddleware(IConfiguration configuration) : IMiddleware
{
    private readonly string[] _adminCommands =
    [
        AdminCommandsConstants.AddInstruction,
        AdminCommandsConstants.File
    ];

    /// <summary>
    /// Обработка процесса до передачи к <see cref="BaseUpdateProcessor"/>'у
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    public async Task<bool> Process(Update update)
    {
        if (update.Message == null)
        {
            return true;
        }

        var message = update.Message!;

        var admins = configuration.GetSection("secret:admins").Get<long[]>();
        var sender = message.From!;

        if (!_adminCommands.Contains(update.Message.Text))
        {
            return true;
        }

        return admins.Contains(sender.Id);
    }
}