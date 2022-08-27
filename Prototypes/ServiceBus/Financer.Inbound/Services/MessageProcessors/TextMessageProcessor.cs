using Financer.Common;
using Financer.Common.Messaging;
using Financer.Common.Models;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Financer.Telegram.Inbound.Services.MessageProcessors
{
    public interface ITelegramMessageProcessor
    {
        MessageType MessageType { get; }

        Task Process(Message message, CancellationToken cancellationToken = default);
    }

    public class TextMessageProcessor : ITelegramMessageProcessor
    {
        private readonly IServiceBusSender _serviceBusSender;

        public TextMessageProcessor(IServiceBusSender serviceBusSender)
        {
            _serviceBusSender = serviceBusSender;
        }

        public MessageType MessageType => MessageType.Text;

        public async Task Process(Message message, CancellationToken cancellationToken = default)
        {
            var messageText = message?.Text?.ToUpper();
            if (string.IsNullOrEmpty(messageText)) return;
            if (messageText == "/START")
            {
                await _serviceBusSender.Send(Queues.TelegramOutbound.Name, 
                    new TelegramOutboundMessage()
                    {
                        ConversationContext = new ()
                        {
                            Chat = message.Chat,
                            MessageId = message.MessageId
                        },
                        MessageText = "Hello"
                    });
                return;
            }

            await _serviceBusSender.Send(Queues.TextMessage.Name, new UserMessage()
            {
                MessageText = message.Text,
                SourceContext = new { message.Chat, message.MessageId }
            });

            await _serviceBusSender.Send(Queues.TelegramOutbound.Name,
                new TelegramOutboundMessage
                {
                    MessageText = "Accepted for processing.",
                    ConversationContext = new() 
                    { 
                        Chat = message.Chat, 
                        MessageId = message.MessageId 
                    }
                });
        }
    }
}