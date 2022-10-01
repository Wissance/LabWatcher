using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            _smtpClient = new SmtpClient(_config.NotificationSettings.MailSettings.Host, _config.NotificationSettings.MailSettings.Port);
        }

        public async Task<bool> NotifySpectrumSavedAsync(IList<SpectrumReadyData> spectra)
        {
            try
            {
                string recipients = String.Join(",", _config.NotificationSettings.MailSettings.RecipientsEMails);
                MailMessage msg = new MailMessage(_config.NotificationSettings.MailSettings.SenderEMail, recipients);
                msg.IsBodyHtml = true;
                // todo: load from template
                msg.Subject = SpectrumAutoSaveMailSubject;
                string mailTemplate = await File.ReadAllTextAsync(Path.GetFullPath(SpectrumAutoSaveMailTemplate));
                // prepare 
                msg.Body = FormatMailMessage(mailTemplate, spectra);

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
            return mailMessage;
        }

        private const int MaxAllowedTimeout = 5000;
        private const string SpectrumAutoSaveMailSubject = "Автоматически сохраненные спектры";
        private const string SpectrumAutoSaveMailTemplate = @"Templates/autosaveNotifications.html";

        private const string CurrentSatePlaceholder = "{currDate}";

        private readonly ApplicationConfig _config;
        private readonly ILogger<EmailNotifier> _logger;
        private readonly SmtpClient _smtpClient;
    }
}
