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

        public TelegramSendRequisites(string botKey, long? groupId, string groupName = null)
        {
            if (groupId == null && string.IsNullOrEmpty(groupName))
                throw new ArgumentException("Group id and name are both null, it is required to provide a value for one of them");
            BotKey = botKey;
            GroupId = groupId;
            GroupName = groupName;
        }

        public long? GroupId { get; set; }
        public string GroupName { get; set; }
        public string BotKey { get; set; }
    }
}
