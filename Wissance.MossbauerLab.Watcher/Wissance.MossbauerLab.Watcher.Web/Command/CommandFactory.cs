using Telegram.Bot;
using Telegram.Bot.Types;
using Wissance.MossbauerLab.Watcher.Common.Data.Notification;
using Wissance.MossbauerLab.Watcher.Data;
using Wissance.MossbauerLab.Watcher.Web.Config;

namespace Wissance.MossbauerLab.Watcher.Web.Command
{
    public static class CommandFactory
    {
        public static ICommand Create(string command, ITelegramBotClient botClient, ModelContext context,  ChatId chat, 
            CommandAnswerConfig commandAnswerConfig)
        {
            switch (command.ToLower())
            {
                case CommandDefs.StartCmd:
                    return new StartCommand(commandAnswerConfig.StartCmdAnswer, botClient, chat);
                case CommandDefs.HelpCmd:
                    return new HelpCommand(commandAnswerConfig.HelpCmdAnswer, botClient, chat);
                default:
                    return null;
            }
        }
        
    }
}