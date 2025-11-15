using System;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace Wissance.MossbauerLab.Watcher.Web.Command
{
    public class HelpCommand : ICommand
    {
        public HelpCommand(CommandContext context)
        {
            _context = context;
        }
        public async Task<bool> ExecuteAsync(string[] parameters)
        {
            string path = Path.GetFullPath(_context.Config.HelpCmdAnswer);
            if (!File.Exists(path))
            {
                return false;
            }

            string helpMessage = await File.ReadAllTextAsync(path);
            await _context.BotClient.SendTextMessageAsync(_context.RawMessage.Chat.Id, helpMessage);
            return true;
        }

        private readonly CommandContext _context;
    }
}