using System.Threading;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Wissance.MossbauerLab.Watcher.Data;
using Wissance.MossbauerLab.Watcher.Web.Config;

namespace Wissance.MossbauerLab.Watcher.Web.Command
{
    public class CommandContext
    {
        public CommandContext()
        {
        }

        public CommandContext(string command, ITelegramBotClient botClient, ModelContext context, Message rawMessage,
            CommandAnswerConfig commandAnswerConfig, CancellationToken token, ILoggerFactory loggerFactory)
        {
            Command = command;
            BotClient = botClient;
            Context = context;
            RawMessage = rawMessage;
            Config = commandAnswerConfig;
            Token = token;
            LoggerFactory = loggerFactory;
        }

        public string Command { get; set; }
        public ITelegramBotClient BotClient { get; set; }
        public ModelContext Context { get; set; }
        public Message RawMessage { get; set; }
        public CommandAnswerConfig Config { get; set; }
        public CancellationToken Token { get; set; }
        public ILoggerFactory LoggerFactory { get; set; }
    }
}