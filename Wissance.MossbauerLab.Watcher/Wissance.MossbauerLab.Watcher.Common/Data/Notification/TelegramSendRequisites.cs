using System;
using System.Collections.Generic;
using System.Text;

namespace Wissance.MossbauerLab.Watcher.Common.Data.Notification
{
    public class TelegramSendRequisites
    {
        public TelegramSendRequisites()
        {
        }

        public TelegramSendRequisites(string group, string templateFilePath)
        {
            Group = group;
            TemplateFilePath = templateFilePath;
        }

        public string Group { get; set; }
        public string TemplateFilePath { get; internal set; }
    }
}
