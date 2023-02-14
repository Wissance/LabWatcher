using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wissance.MossbauerLab.Watcher.Services.Config
{
    public class NotificationConfig
    {
        public NotificationConfig()
        {

        }

        public NotificationConfig(MailConfig mailSettings, int threshold, TelegramConfig telegramSettings)
        {
            MailSettings = mailSettings;
            Threshold = threshold;
            TelegramSettings = telegramSettings;
        }

        public MailConfig MailSettings { get; set; }
        public TelegramConfig TelegramSettings { get; set; }
        public int Threshold { get; set; }
    }
}
