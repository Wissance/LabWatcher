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
            switch (context.Command)
            {
                case CommandAnswerLocalizationDefs.StartCmd:
                    return new StartCommand(context);
                case CommandAnswerLocalizationDefs.HelpCmd:
                    return new HelpCommand(context);
                case CommandAnswerLocalizationDefs.ListSpectraCmd:
                    return new ListSpectraCommand(context);
                case CommandAnswerLocalizationDefs.GetSpectrumInfoCmd:
                    return new GetSpectrumDetailsCommand(context);
                case CommandAnswerLocalizationDefs.CheckStateCmd:
                    return new CheckStateCommand(context);
                default:
                    return null;
            }
        }
        
    }
}