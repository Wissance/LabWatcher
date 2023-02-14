using System;
using System.Collections.Generic;
using System.Text;

namespace Wissance.MossbauerLab.Watcher.Common.Data.Notification
{
    public class MailSendRequisites
    {
        public MailSendRequisites()
        {

        }

        public MailSendRequisites(string host, int port, string sender, string password, string[] recipients)
        {
            Host = host;
            Port = port;
            Sender = sender;
            Recipients = recipients;
            Password = password;
        }

        public string Host { get; set; }
        public int Port { get; set; }
        public string Sender { get; set; }
        public string Password { get; set; }
        public string[] Recipients { get; set; }
    }
}
