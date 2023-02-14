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
        private readonly ApplicationConfig _config;
        private const string SpectrumAutoSaveMailTemplate = @"Templates/autosaveNotifications.html";

        public TelegramNotifier(ApplicationConfig config)
        {
            _config = config;
        }
        public async Task<bool> NotifySpectrumSavedAsync(IList<SpectrumReadyData> spectra)
        {
            ITelegramBotClient client = new TelegramBotClient("6253527316:AAHrOysU6QKubqCg51aHuVCNBf3PtD3nGBU");
           
            var targetGroupName = _config.NotificationSettings.TelegramSettings.NotificationGroupName;
            ChatId targetChatId = new ChatId(targetGroupName);

            var msg = new Message();
            string mailTemplate = System.IO.File.ReadAllText(_config.NotificationSettings.TelegramSettings.TemplateFilePath);
            msg.Text = NotificationMessageFormatter.FormatTelegramMessage(mailTemplate, spectra);
            
            try
            {
                await client.SendTextMessageAsync(targetChatId, msg.Text);
            }
            catch (Exception)
            {

                return false;
            }
            return true;
        }
    }
}
