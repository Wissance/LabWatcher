using Wissance.MossbauerLab.Watcher.Common.Data.Notification;

namespace Wissance.MossbauerLab.Watcher.Web.Config
{
    public class TelegramConfig : TelegramSendRequisites
    {
        public TelegramConfig()
        {
        }

        public TelegramConfig(string key, long? groupId, string groupName = null)
           : base( key, groupId, groupName)
        {

        }
    }
}