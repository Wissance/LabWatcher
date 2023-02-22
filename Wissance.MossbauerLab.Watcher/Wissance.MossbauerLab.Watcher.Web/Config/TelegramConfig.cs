using Wissance.MossbauerLab.Watcher.Common.Data.Notification;

namespace Wissance.MossbauerLab.Watcher.Web.Config
{
    public class TelegramConfig : TelegramSendRequisites
    {
        public TelegramConfig()
        {
        }

        public TelegramConfig(string group, string key, string templateFilePath)
           :base(group, key, templateFilePath)
        {

        }
    }
}