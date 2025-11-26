namespace Wissance.MossbauerLab.Watcher.Web.Config
{
    public class NotificationConfig
    {
        public NotificationConfig()
        {

        }

        public NotificationConfig(TemplatesConfig templates, CommandAnswerConfig commandAnswer, MailConfig mailSettings, 
            TelegramConfig telegramSettings, int threshold)
        {
            Templates = templates;
            CommandAnswer = commandAnswer;
            MailSettings = mailSettings;
            Threshold = threshold;
            TelegramSettings = telegramSettings;
        }
        public TemplatesConfig Templates { get; set; }
        public CommandAnswerConfig CommandAnswer { get; set; }
        public MailConfig MailSettings { get; set; }
        public TelegramConfig TelegramSettings { get; set; }
        public int Threshold { get; set; }
    }
}
