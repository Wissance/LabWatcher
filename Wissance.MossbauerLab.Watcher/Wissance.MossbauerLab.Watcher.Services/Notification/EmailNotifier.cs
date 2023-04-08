using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Wissance.MossbauerLab.Watcher.Common;
using Wissance.MossbauerLab.Watcher.Common.Data;
using Wissance.MossbauerLab.Watcher.Common.Data.Notification;

namespace Wissance.MossbauerLab.Watcher.Services.Notification
{
    public class EmailNotifier : ISpectrumMeasureEventsNotifier
    {
        public EmailNotifier(MailSendRequisites mailRequisites, TemplateManager templateManager, ILoggerFactory loggerFactory)
        {
            _mailRequisites = mailRequisites;
            _logger = loggerFactory.CreateLogger<EmailNotifier>();
            if (templateManager == null)
                throw new ArgumentNullException("templateManager");
            _templateManager = templateManager;
            _smtpClient = new SmtpClient(_mailRequisites.Host, _mailRequisites.Port)
            {
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_mailRequisites.Sender, _mailRequisites.Password)
            };
        }

        public async Task<bool> NotifySpectrumSavedAsync(IList<SpectrumReadyData> spectra)
        {
            try
            {
                string recipients = string.Join(",", _mailRequisites.Recipients);
                MailMessage msg = new MailMessage(_mailRequisites.Sender, recipients);
                msg.IsBodyHtml = true;
                msg.Subject = SpectrumAutoSaveMailSubject;
                if (!_templateManager.Templates.ContainsKey(SpectrometerEvent.SpectrumSaved))
                    throw new InvalidDataException("Expected that key \"SpectrometerEvent.SpectrumSaved\" present in _templates, actually not");
                string template = _templateManager.Templates[SpectrometerEvent.SpectrumSaved].PositiveCase;
                string mailTemplate = await File.ReadAllTextAsync(Path.GetFullPath(template));
                // prepare 
                msg.Body = NotificationMessageFormatter.FormatMailMessage(mailTemplate, spectra);
                foreach (SpectrumReadyData spec in spectra)
                {
                    Stream stream = new MemoryStream(spec.Spectrum);
                    msg.Attachments.Add(new Attachment(stream, $"{spec.Name}.spc"));
                }
                _smtpClient.Send(msg);


                await Task.WhenAny(new Task[]
                {
                    new Task(() => _smtpClient.Send(msg)),
                    Task.Delay(MaxAllowedTimeout)
                });

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during sending e-mail: {e.Message}");
                return false;
            }

        }

        public string FormatMailMessage(string template, IList<SpectrumReadyData> spectra)
        {
            string mailMessage = template.Replace(CurrentSatePlaceholder, DateTime.Now.ToString("yyyy-MM-dd:HH-mm-ss"));
            IList<string> lines = spectra.Select(s => string.Format(SavedSpectrumDescriptionTemplate, s.Name, s.Channel, s.RawInfo.LastWriteTime.ToString("yyyy-MM-dd:HH-mm-ss"))).ToList();
            string linesStr = string.Join(Environment.NewLine, lines);
            mailMessage = mailMessage.Replace(AutosavedSpectraPlaceholder, linesStr);

            return mailMessage;
        }

        private const int MaxAllowedTimeout = 10000;
        private const string SpectrumAutoSaveMailSubject = "Автоматически сохраненные спектры";
        // private const string SpectrumAutoSaveMailTemplate = @"Notification/Templates/autosaveNotifications.html";

        private const string CurrentSatePlaceholder = "{currDate}";
        private const string AutosavedSpectraPlaceholder = "{savedSpectra}";
        // <!--<li>Спектр {msSpName} по каналу {msChNumber} сохранен {msSaveDate}</li>-->
        private const string SavedSpectrumDescriptionTemplate = "<li>Спектр {0} по каналу {1} сохранен {2}</li>";

        private readonly MailSendRequisites _mailRequisites;
        private readonly ILogger<EmailNotifier> _logger;
        private readonly SmtpClient _smtpClient;
        private readonly TemplateManager _templateManager;
    }
}
