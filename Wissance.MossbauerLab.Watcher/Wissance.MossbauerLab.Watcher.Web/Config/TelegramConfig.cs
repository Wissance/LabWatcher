using Wissance.MossbauerLab.Watcher.Common.Data.Notification;

namespace Wissance.MossbauerLab.Watcher.Web.Config
{
    public class TelegramConfig : TelegramSendRequisites
    {
        public TelegramConfig()
        {
        }

        public TelegramConfig(string key, string templateFilePath, long? groupId, string groupName = null)
           : base( key, templateFilePath, groupId, groupName)
        {

        }
    }
}