using Azure.Messaging.ServiceBus;
using Financer.Common.Serialization;
using Microsoft.Extensions.Configuration;

namespace Financer.Common.Messaging
{
    public interface IServiceBusSender
    {
        Task Send<TMessage>(string queueName, TMessage message);
    }

    public class ServiceBusSender : IServiceBusSender
    {
        private readonly IConfiguration _configuration;
        private readonly ServiceBusClient _client;

        public ServiceBusSender(ServiceBusClient client,
            IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
        }

        public async Task Send<TMessage>(string queueName, TMessage message)
        {
            await using var sender = _client.CreateSender(_configuration[queueName]);
            using var messageBatch = await sender.CreateMessageBatchAsync();
            var messageText = message.Serialize();
            if (!messageBatch.TryAddMessage(new ServiceBusMessage(messageText)))
            {
                throw new InvalidOperationException("The message is too large to fit in the batch.");
            }
            await sender.SendMessagesAsync(messageBatch);
        }
    }
}
