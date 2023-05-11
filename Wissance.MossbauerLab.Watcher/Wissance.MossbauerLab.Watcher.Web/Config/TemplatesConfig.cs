using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wissance.MossbauerLab.Watcher.Web.Config
{
    public class MailTemplates
    {
        public string AutosaveDone { get; set; }
    }

    public class TelegramTemplates
    {
        public string AutosaveDone { get; set; }
        public string AutosaveEmpty { get; set; }
    }

    public class TemplatesConfig
    {
        public MailTemplates Mail { get; set; }
        public TelegramTemplates Telegram { get; set; }
    }
}
