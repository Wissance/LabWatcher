using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

using Wissance.MossbauerLab.Watcher.Common.Data;
using Wissance.MossbauerLab.Watcher.Services.Config;

namespace Wissance.MossbauerLab.Watcher.Services.Notification
{
    public class TelegramNotifier : ISpectrumReadyNotifier
    {
     
        public TelegramNotifier(ApplicationConfig config, ILoggerFactory loggerFactory)
        {
            _config = config;
            _logger = loggerFactory.CreateLogger<TelegramNotifier>();
        }
        public async Task<bool> NotifySpectrumSavedAsync(IList<SpectrumReadyData> spectra)
        {
            ITelegramBotClient client = new TelegramBotClient("6253527316:AAHrOysU6QKubqCg51aHuVCNBf3PtD3nGBU");
           
            string targetGroupName = _config.NotificationSettings.TelegramSettings.NotificationGroupName;
            ChatId targetChatId = new ChatId(targetGroupName);

            Message msg = new Message();
            string mailTemplate = System.IO.File.ReadAllText(_config.NotificationSettings.TelegramSettings.TemplateFilePath);
            msg.Text = NotificationMessageFormatter.FormatTelegramMessage(mailTemplate, spectra);
            
            try
            {
                await client.SendTextMessageAsync(targetChatId, msg.Text);
            }
            catch (Exception e )
            {
                _logger.LogError($"An error occurred during sending message to telegram group {targetGroupName}: {e.Message}");
                return false;
            }
            return true;
        }
        
        private readonly ApplicationConfig _config;
        private readonly ILogger<TelegramNotifier> _logger;

    }
}
