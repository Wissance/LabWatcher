namespace Wissance.MossbauerLab.Watcher.Web.Config
{
    public class NotificationConfig
    {
        public NotificationConfig()
        {

        }

        public NotificationConfig(TemplatesConfig templates, MailConfig mailSettings, TelegramConfig telegramSettings, int threshold)
        {
            Templates = templates;
            MailSettings = mailSettings;
            Threshold = threshold;
            TelegramSettings = telegramSettings;
        }
        public TemplatesConfig Templates { get; set; }
        public MailConfig MailSettings { get; set; }
        public TelegramConfig TelegramSettings { get; set; }
        public int Threshold { get; set; }
    }
}
