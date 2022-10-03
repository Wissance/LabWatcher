﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wissance.MossbauerLab.Watcher.Web.Config;
using Microsoft.Extensions.Logging;
using Wissance.MossbauerLab.Watcher.Web.Data;

namespace Wissance.MossbauerLab.Watcher.Web.Services.Notification
{
    public class EmailNotifier : ISpectrumReadyNotifier
    {
        public EmailNotifier(ApplicationConfig config, ILoggerFactory loggerFactory)
        {
            _config = config;
            _logger = loggerFactory.CreateLogger<EmailNotifier>();
            _smtpClient = new SmtpClient(_config.NotificationSettings.MailSettings.Host, _config.NotificationSettings.MailSettings.Port)
            {
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_config.NotificationSettings.MailSettings.Sender, _config.NotificationSettings.MailSettings.Password)
            };
        }

        public async Task<bool> NotifySpectrumSavedAsync(IList<SpectrumReadyData> spectra)
        {
            try
            {
                string recipients = String.Join(",", _config.NotificationSettings.MailSettings.Recipients);
                MailMessage msg = new MailMessage(_config.NotificationSettings.MailSettings.Sender, recipients);
                msg.IsBodyHtml = true;
                msg.Subject = SpectrumAutoSaveMailSubject;
                string mailTemplate = await File.ReadAllTextAsync(Path.GetFullPath(SpectrumAutoSaveMailTemplate));
                // prepare 
                msg.Body = FormatMailMessage(mailTemplate, spectra);
                foreach (SpectrumReadyData spec in spectra)
                {
                    Stream stream = new MemoryStream(spec.Spectrum);
                    msg.Attachments.Add(new Attachment(stream , spec.Name));
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
            string mailMessage = template.Replace(CurrentSatePlaceholder, DateTime.Now.ToString("F"));
            IList<string> lines = spectra.Select(s => string.Format(SavedSpectrumDescriptionTemplate, s.Name, s.Channel, s.RawInfo.LastWriteTime)).ToList();
            string linesStr = string.Join(Environment.NewLine, lines);
            mailMessage = mailMessage.Replace(AutosavedSpectraPlaceholder, linesStr);

            return mailMessage;
        }

        private const int MaxAllowedTimeout = 10000;
        private const string SpectrumAutoSaveMailSubject = "Автоматически сохраненные спектры";
        private const string SpectrumAutoSaveMailTemplate = @"Templates/autosaveNotifications.html";

        private const string CurrentSatePlaceholder = "{currDate}";
        private const string AutosavedSpectraPlaceholder = "{savedSpectra}";
        // <!--<li>Спектр {msSpName} по каналу {msChNumber} сохранен {msSaveDate}</li>-->
        private const string SavedSpectrumDescriptionTemplate = "<li>Спектр {0} по каналу {1} сохранен {2}</li>";

        private readonly ApplicationConfig _config;
        private readonly ILogger<EmailNotifier> _logger;
        private readonly SmtpClient _smtpClient;
    }
}
