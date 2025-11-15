using Telegram.Bot;
using Telegram.Bot.Types;
using Wissance.MossbauerLab.Watcher.Common.Data.Notification;
using Wissance.MossbauerLab.Watcher.Data;
using Wissance.MossbauerLab.Watcher.Web.Config;

namespace Wissance.MossbauerLab.Watcher.Web.Command
{
    public static class CommandFactory
    {
        public static ICommand Create(CommandContext context)
        {
            switch (context.Command.ToLower())
            {
                case CommandDefs.StartCmd:
                    return new StartCommand(context);
                case CommandDefs.HelpCmd:
                    return new HelpCommand(context);
                default:
                    return null;
            }
        }
        
    }
}