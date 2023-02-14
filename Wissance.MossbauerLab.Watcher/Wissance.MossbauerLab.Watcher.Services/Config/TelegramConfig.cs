namespace Wissance.MossbauerLab.Watcher.Services.Config
{
    public class TelegramConfig
    {
        public string NotificationGroupName { get; set; }

        public TelegramConfig(string sendNotificationToGroupWithId)
        {
            NotificationGroupName = sendNotificationToGroupWithId;
        }
    }
}