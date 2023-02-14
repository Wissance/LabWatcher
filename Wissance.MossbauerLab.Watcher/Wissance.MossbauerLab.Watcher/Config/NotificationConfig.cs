namespace Wissance.MossbauerLab.Watcher.Web.Config
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
