using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
        public StartCommand(CommandContext context)
        {
            _context = context;
            _logger = context.LoggerFactory.CreateLogger<StartCommand>();
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
                string path = Path.GetFullPath(_context.Config.StartCmdAnswer);
                if (!File.Exists(path))
                {
                    return false;
                }

                string greetingMsg = await File.ReadAllTextAsync(path);
                // 1. Send greeting
                await _context.BotClient.SendTextMessageAsync(_context.RawMessage.Chat.Id, greetingMsg);
                // 2. Send Keyboard
                await _context.BotClient.SendTextMessageAsync(_context.RawMessage.Chat.Id, "Выберите команду", replyMarkup: GetActionKeyboard());
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during the handling /start command, {e.Message}");
                return false;
            }
        }

        private static InlineKeyboardMarkup GetActionKeyboard()
        {
            InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(new List<InlineKeyboardButton>[]
            {
                new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData(CommandDefs.KeyboardCaptions[CommandDefs.ListSpectraCmd], CommandDefs.ListSpectraCmd)
                },
                new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData(CommandDefs.KeyboardCaptions[CommandDefs.GetSpectrumInfoCmd], CommandDefs.GetSpectrumInfoCmd),
                    InlineKeyboardButton.WithCallbackData(CommandDefs.KeyboardCaptions[CommandDefs.GetSpectrumFilesCmd], CommandDefs.GetSpectrumFilesCmd)
                },
                new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData(CommandDefs.KeyboardCaptions[CommandDefs.CheckStateCmd], CommandDefs.CheckStateCmd)
                }
            });
            return keyboard;
        }

        private readonly CommandContext _context;
        private readonly ILogger<StartCommand> _logger;
    }
}