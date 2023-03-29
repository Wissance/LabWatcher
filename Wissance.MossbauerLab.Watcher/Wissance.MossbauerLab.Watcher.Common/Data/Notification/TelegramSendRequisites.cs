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

        public TelegramSendRequisites(long groupId, string botKey, string templateFilePath, string groupName = null)
        {
            GroupId = groupId;
            BotKey = botKey;
            TemplateFilePath = templateFilePath;
            GroupName = groupName;
        }

        public long GroupId { get; set; }
        public string GroupName { get; set; }
        public string BotKey { get; set; }
        public string TemplateFilePath { get; set; }
    }
}
