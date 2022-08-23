using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wissance.MossbauerLab.Watcher.Web.Config;
using Microsoft.Extensions.Logging;

namespace Wissance.MossbauerLab.Watcher.Web.Services.Notification
{
    public class EmailNotifier
    {
        public EmailNotifier(ApplicationConfig config, ILoggerFactory loggerFactory)
        {
            _config = config;
            _logger = loggerFactory.CreateLogger<EmailNotifier>();
            // todo: umv: init mail settings
        }

        //SmtpClient smtp = new SmtpClient(host, port);

        private readonly ApplicationConfig _config;
        private readonly ILogger<EmailNotifier> _logger;
    }
}
