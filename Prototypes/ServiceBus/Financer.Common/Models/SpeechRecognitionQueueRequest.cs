using System;

namespace Financer.Common.Models
{
    public class SpeechRecognitionQueueRequest
    {
        public Guid BlobGuid { get; set; }

        public TelegramConversationContext ConversationContext { get; set; }
    }
}
