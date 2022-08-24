﻿using System;
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
            string from = "watcher@mossblab.com";
            string to = "um.nix.user@gmail.com";
            MailMessage msg = new MailMessage(from, to);
            msg.Subject = "Testing mail delivery";
            msg.Body = "Finally we send here mail on 2 channels";
            //msg.Attachments.Add(new Attachment("D:\\myfile.txt"));
            _smtpClient.Send(msg);
            return true;
        }


        private readonly ApplicationConfig _config;
        private readonly ILogger<EmailNotifier> _logger;
        private readonly SmtpClient _smtpClient;
    }
}
