using Wissance.MossbauerLab.Watcher.Common.Data.Notification;

namespace Wissance.MossbauerLab.Watcher.Web.Config
{
    public class MailConfig : MailSendRequisites
    {
        public MailConfig()
        {

        }

        public MailConfig(string host, int port, string sender, string password, string[] recipients)
            : base(host, port, sender, password, recipients)
        {

        }
    }
}
