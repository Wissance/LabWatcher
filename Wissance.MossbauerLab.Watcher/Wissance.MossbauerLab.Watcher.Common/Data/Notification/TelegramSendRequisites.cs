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

        public TelegramSendRequisites(string group, string botKey, string templateFilePath)
        {
            Group = group;
            BotKey = botKey;
            TemplateFilePath = templateFilePath;
        }

        public string Group { get; set; }
        public string BotKey { get; set; }
        public string TemplateFilePath { get; set; }
    }
}
