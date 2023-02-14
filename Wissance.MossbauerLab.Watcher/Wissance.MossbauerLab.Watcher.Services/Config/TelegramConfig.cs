namespace Wissance.MossbauerLab.Watcher.Services.Config
{
    public class TelegramConfig
    {
        public string NotificationGroupName { get; set; }
        public string TemplateFilePath { get; internal set; }

        public TelegramConfig(string sendNotificationToGroupWithName, string templateFilePath)
        {
            NotificationGroupName = sendNotificationToGroupWithName;
            TemplateFilePath = templateFilePath;
        }
    }
}