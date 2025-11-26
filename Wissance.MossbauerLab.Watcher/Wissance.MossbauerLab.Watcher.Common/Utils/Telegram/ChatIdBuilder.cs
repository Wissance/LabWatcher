using Telegram.Bot.Types;
using Wissance.MossbauerLab.Watcher.Common.Data.Notification;

namespace Wissance.MossbauerLab.Watcher.Common.Utils.Telegram
{
    public static class ChatIdBuilder
    {
        public static ChatId Build(TelegramSendRequisites tgRequisites)
        {
            // This is tutorial about where to get chatId : https://gist.github.com/nafiesl/4ad622f344cd1dc3bb1ecbe468ff9f8a
            if (tgRequisites.GroupId.HasValue)
                return new ChatId(tgRequisites.GroupId.Value);
            return new ChatId(tgRequisites.GroupName);
        }
    }
}