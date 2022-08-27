using Telegram.Bot.Types;

namespace Financer.Common.Models
{
    public class TelegramConversationContext
    {
        public int? MessageId { get; set; }

        public Chat Chat { get; set; }
    }

    public class TelegramOutboundMessage
    {
        public string MessageText { get; set; }

        public TelegramConversationContext ConversationContext { get; set; }
    }
}
