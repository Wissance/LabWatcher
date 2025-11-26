using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace Wissance.MossbauerLab.Watcher.Web.Command
{
    public class HelpCommand : ICommand
    {
        public HelpCommand(CommandContext context)
        {
            _context = context;
            _logger = _context.LoggerFactory.CreateLogger<HelpCommand>();
        }
        public async Task<bool> ExecuteAsync(string[] parameters)
        {
            try
            {
                string path = Path.GetFullPath(_context.Config.NotificationSettings.CommandAnswer.HelpCmdAnswer);
                if (!File.Exists(path))
                {
                    return false;
                }

                string helpMessage = await File.ReadAllTextAsync(path);
                await _context.BotClient.SendTextMessageAsync(_context.RawMessage.Chat.Id, helpMessage,
                    ParseMode.Markdown);
                return true;

            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during the handling {_context.Command} command, {e.Message}");
                return false;
            }
        }

        private readonly CommandContext _context;
        private readonly ILogger<HelpCommand> _logger;
    }
}