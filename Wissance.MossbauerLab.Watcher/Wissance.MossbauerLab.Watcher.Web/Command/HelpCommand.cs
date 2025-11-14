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
        public HelpCommand(string answerFilePath, ITelegramBotClient botClient, ChatId chat)
        {
            _answerFilePath = answerFilePath;
            _botClient = botClient;
            _chat = chat;
        }
        public async Task<bool> ExecuteAsync(string[] parameters)
        {
            string path = Path.GetFullPath(_answerFilePath);
            if (!File.Exists(path))
            {
                return false;
            }

            string message = await File.ReadAllTextAsync(Path.GetFullPath(_answerFilePath));
            return true;
        }
        
        private readonly string _answerFilePath;
        private readonly ITelegramBotClient _botClient;
        private readonly ChatId _chat;
    }
}