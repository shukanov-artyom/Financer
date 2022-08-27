using Azure.Messaging.ServiceBus;

namespace Financer.Consumer
{
    public class ConsumerWorker : BackgroundService
    {
        private readonly ServiceBusProcessor _processor;

        public ConsumerWorker(ServiceBusClient client,
            IConfiguration configuration)
        {
            var queueName = configuration["ServiceBusQueueName"];
            _processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());

            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _processor.StartProcessingAsync();
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(5000, stoppingToken);
            }
            await _processor.StopProcessingAsync();
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Received: {body}");

            // complete the message. messages is deleted from the queue. 
            await args.CompleteMessageAsync(args.Message);
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
