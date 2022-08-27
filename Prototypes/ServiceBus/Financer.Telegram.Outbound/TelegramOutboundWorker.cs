using Azure.Messaging.ServiceBus;
using Financer.Common.Models;
using Financer.Telegram.Outbound.Infrastructure;
using System.Text.Json;
using Telegram.Bot;

namespace Financer.Telegram.Outbound
{
    public class TelegramOutboundWorker : BackgroundService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ServiceBusProcessor _processor;

        public TelegramOutboundWorker(ServiceBusClient serviceBusClient,
            ITelegramBotClient botClient,
            IConfiguration configuration)
        {
            _botClient = botClient;

            var queueName = configuration["ServiceBusQueueName"];
            _processor = serviceBusClient.CreateProcessor(queueName, 
                new ServiceBusProcessorOptions());

            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            
            var outboundMessage = JsonSerializer.Deserialize<TelegramOutboundMessage>(body,
                new JsonSerializerOptions() { PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance });

            await _botClient.SendTextMessageAsync(
                outboundMessage.ConversationContext.Chat, 
                outboundMessage.MessageText, 
                replyToMessageId: outboundMessage.ConversationContext.MessageId);
            
            await args.CompleteMessageAsync(args.Message);
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
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
    }
}
