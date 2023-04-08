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
using Wissance.MossbauerLab.Watcher.Common;
using Wissance.MossbauerLab.Watcher.Common.Data;
using Wissance.MossbauerLab.Watcher.Common.Data.Notification;

namespace Wissance.MossbauerLab.Watcher.Services.Notification
{
    public class TelegramNotifier : ISpectrumMeasureEventsNotifier
    {
     
        public TelegramNotifier(TelegramSendRequisites tgRequisites, IDictionary<SpectrometerEvent, MessageTemplate> templates, ILoggerFactory loggerFactory)
        {
            _tgRequisites = tgRequisites;
            if (templates == null)
                throw new ArgumentNullException("templates");
            _templates = templates;
            _logger = loggerFactory.CreateLogger<TelegramNotifier>();
        }
        public async Task<bool> NotifySpectrumSavedAsync(IList<SpectrumReadyData> spectra)
        {
            string username = "";
            try
            {
                ITelegramBotClient client = new TelegramBotClient(_tgRequisites.BotKey);
                ChatId targetChatId = GetChatId(_tgRequisites);
                Message msg = CreateMessageFromTemplate(spectra);
                username = targetChatId.Username;
                await client.SendTextMessageAsync(targetChatId, msg.Text);
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during sending message to telegram group: {username}: {e.Message}");
                return false;
            }
            return true;
        }

        private Message CreateMessageFromTemplate(IList<SpectrumReadyData> spectra)
        {
            Message msg = new Message();
            if (!_templates.ContainsKey(SpectrometerEvent.SpectrumSaved))
                throw new InvalidDataException("Expected that key \"SpectrometerEvent.SpectrumSaved\" present in _templates, actually not");
            string template;
            bool spectraIsEmpty = !spectra.Any();
            if (spectraIsEmpty)
            {
                template = _templates[SpectrometerEvent.SpectrumSaved].PositiveCase;
            }
            else
            {
                template = _templates[SpectrometerEvent.SpectrumSaved].NegativeCase;
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

        private readonly IDictionary<SpectrometerEvent, MessageTemplate> _templates;
        private readonly TelegramSendRequisites _tgRequisites;
        private readonly ILogger<TelegramNotifier> _logger;
    }
}
