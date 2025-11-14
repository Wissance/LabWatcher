using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Wissance.MossbauerLab.Watcher.Common.Data.Notification;
using File = System.IO.File;

namespace Wissance.MossbauerLab.Watcher.Web.Command
{
    // todo(UMV) : pass logger factory
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
                // 1. Send greeting
                await _botClient.SendTextMessageAsync(_chat, greetingMsg);
                // 2. Send Keyboard
                //_botClient.SendTextMessageAsync(_chat, new K)
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private static InlineKeyboardMarkup GetActionKeyboard()
        {
            InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(new List<InlineKeyboardButton>[]
            {
                new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData("", CommandDefs.ListSpectraCmd)
                }
            });
            return keyboard;
        }

        private readonly string _answerFilePath;
        private readonly ITelegramBotClient _botClient;
        private readonly ChatId _chat;
    }
}