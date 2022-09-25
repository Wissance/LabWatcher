using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Wissance.MossbauerLab.Watcher.Web.Config
{
    public class MailConfig
    {
        public MailConfig()
        {

        }

        public MailConfig(string host, int port, string senderEMail, string recipientsEMails)
        {
            Host = host;
            Port = port;
            SenderEMail = senderEMail;
            RecipientsEMails = recipientsEMails;
        }

        public string Host { get; set; }
        public int Port { get; set; }
        public string SenderEMail { get; set; }
        public string RecipientsEMails { get; set; }
    }
}
