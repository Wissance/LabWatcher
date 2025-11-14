using System;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Wissance.MossbauerLab.Watcher.Common.Data.Notification;
using File = System.IO.File;

namespace Wissance.MossbauerLab.Watcher.Web.Command
{
    public class StartCommand : ICommand
    {
        public StartCommand(string answerFilePath, ITelegramBotClient botClient, ChatId chat)
        {
            _answerFilePath = answerFilePath;
            _botClient = botClient;
            _chat = chat;
        }
        /// <summary>
        ///    This is command executing when user send /start to tg bot
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<bool> ExecuteAsync(string[] parameters)
        {
            try
            {
                string path = Path.GetFullPath(_answerFilePath);
                if (!File.Exists(path))
                {
                    return false;
                }

                string greetingMsg = await File.ReadAllTextAsync(Path.GetFullPath(_answerFilePath));
                await _botClient.SendTextMessageAsync(_chat, greetingMsg);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private readonly string _answerFilePath;
        private readonly ITelegramBotClient _botClient;
        private readonly ChatId _chat;
    }
}