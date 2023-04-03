using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
            ChatId targetChatId = GetChatId(_tgRequisites);
            Message msg = CreateMessageFromTemplate(spectra);

            try
            {
                await client.SendTextMessageAsync(targetChatId, msg.Text);
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during sending message to telegram group {targetChatId.Username}: {e.Message}");
                return false;
            }
            return true;
        }

        private Message CreateMessageFromTemplate(IList<SpectrumReadyData> spectra)
        {
            Message msg = new Message();
            string templateNormal = !string.IsNullOrEmpty(_tgRequisites.TemplateFilePath) ? _tgRequisites.TemplateFilePath : DefaultSpectrumAutoSaveMsgTemplate;
            string templateEmpty = !string.IsNullOrEmpty(_tgRequisites.TemplateFilePathEmptySpectra) ? _tgRequisites.TemplateFilePathEmptySpectra : DefaultEmptySpectrumMsgTemplate;
            string template = templateNormal;
            bool spectraIsEmpty = !spectra.Any();
            if (spectraIsEmpty)
            {
                template = templateEmpty;
            }
            else
            {
                template = templateNormal;
            }
            string mailTemplate = System.IO.File.ReadAllText(template);
            msg.Text = NotificationMessageFormatter.FormatTelegramMessage(mailTemplate, spectra);
            return msg;
        }

        private ChatId GetChatId(TelegramSendRequisites _tgRequisites)
        {
            if (_tgRequisites.GroupId.HasValue)
                return new ChatId(_tgRequisites.GroupId.Value);
            return new ChatId(_tgRequisites.GroupName);
        }

        private const string DefaultSpectrumAutoSaveMsgTemplate = @"Notification/Templates/tgAutosaveDone.txt";
        private const string DefaultEmptySpectrumMsgTemplate = @"Notification/Templates/tgSpectraIsEmpty.txt";
        private readonly TelegramSendRequisites _tgRequisites;
        private readonly ILogger<TelegramNotifier> _logger;

    }
}
