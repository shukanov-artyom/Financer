using System;

namespace Financer.Common
{
    public class Queues
    {
        public Queues(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public static Queues TextMessage { get; } = new("TextMessageServiceBusQueueName");

        public static Queues TelegramOutbound { get; } = new("TelegramOutboundServiceBusQueueName");

        public static Queues SpeechRecognitionInput { get; } = new("SpeechRecognitionInputServiceBusQueueName");
    }
}
