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

        public MailConfig(string host, int port)
        {
            Host = host;
            Port = port;
        }

        public string Host { get; set; }
        public int Port { get; set; }
    }
}
