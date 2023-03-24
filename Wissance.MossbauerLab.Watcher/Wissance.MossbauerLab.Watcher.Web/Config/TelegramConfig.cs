using Wissance.MossbauerLab.Watcher.Common.Data.Notification;

namespace Wissance.MossbauerLab.Watcher.Web.Config
{
    public class TelegramConfig : TelegramSendRequisites
    {
        public TelegramConfig()
        {
        }

        public TelegramConfig(long groupId, string key, string templateFilePath)
           :base(groupId, key, templateFilePath)
        {

        }
    }
}