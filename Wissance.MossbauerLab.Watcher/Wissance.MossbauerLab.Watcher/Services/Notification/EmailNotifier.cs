using System;
using System.Collections.Generic;
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
            _smtpClient = new SmtpClient(_config.MailSettings.Host, _config.MailSettings.Port);
        }

        public async Task<bool> NotifySpectrumSavedAsync(IList<SpectrumReadyData> spectra)
        {
            try
            {
                MailMessage msg = new MailMessage(_config.MailSettings.SenderMail, _config.MailSettings.SenderMail);
                // todo: load from template
                msg.Subject = "Testing mail delivery";
                msg.Body = "Finally we send here mail on 2 channels";

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

        private const int MaxAllowedTimeout = 5000;

        private readonly ApplicationConfig _config;
        private readonly ILogger<EmailNotifier> _logger;
        private readonly SmtpClient _smtpClient;
    }
}
