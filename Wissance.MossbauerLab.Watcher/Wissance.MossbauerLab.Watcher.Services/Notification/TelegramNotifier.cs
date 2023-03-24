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
using Wissance.MossbauerLab.Watcher.Common.Data.Notification;

namespace Wissance.MossbauerLab.Watcher.Services.Notification
{
    public class TelegramNotifier : ISpectrumReadyNotifier
    {
     
        public TelegramNotifier(TelegramSendRequisites tgRequisites, ILoggerFactory loggerFactory)
        {
            _tgRequisites = tgRequisites;
            _logger = loggerFactory.CreateLogger<TelegramNotifier>();
        }
        public async Task<bool> NotifySpectrumSavedAsync(IList<SpectrumReadyData> spectra)
        {
            ITelegramBotClient client = new TelegramBotClient(_tgRequisites.BotKey);
           
            long targetGroupId = _tgRequisites.GroupId;
            ChatId targetChatId = new ChatId(targetGroupId);

            Message msg = new Message();
            string template = !string.IsNullOrEmpty(_tgRequisites.TemplateFilePath) ? _tgRequisites.TemplateFilePath : DefaultSpectrumAutoSaveMailTemplate;
            string mailTemplate = System.IO.File.ReadAllText(template);
            msg.Text = NotificationMessageFormatter.FormatTelegramMessage(mailTemplate, spectra);
            
            try
            {
                 await client.SendTextMessageAsync(targetChatId, msg.Text);
            }
            catch (Exception e )
            {
                _logger.LogError($"An error occurred during sending message to telegram group {targetGroupId}: {e.Message}");
                return false;
            }
            return true;
        }

        private const string DefaultSpectrumAutoSaveMailTemplate = @"Notification/Templates/tgAutosaveDone.txt";
        private readonly TelegramSendRequisites _tgRequisites;
        private readonly ILogger<TelegramNotifier> _logger;

    }
}
