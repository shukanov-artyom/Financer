using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Financer.Common;
using Financer.Common.Models;
using Financer.Common.Serialization;
using Financer.SpeechRecognition.Models;
using Financer.SpeechRecognition.Services;

namespace Financer.SpeechRecognition
{
    public class SpeechRecognitionEnqueueWorker : BackgroundService
    {
        private readonly ServiceBusProcessor _processor;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IBatchTranscriptionService _batchTranscriptionService;

        public SpeechRecognitionEnqueueWorker(ServiceBusClient serviceBusClient,
            BlobServiceClient blobServiceClient,
            IConfiguration configuration,
            IBatchTranscriptionService batchTranscriptionService)
        {
            var queueName = configuration[Queues.SpeechRecognitionInput.Name];
            _processor = serviceBusClient.CreateProcessor(queueName,
                new ServiceBusProcessorOptions());

            _blobServiceClient = blobServiceClient;

            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;
            _batchTranscriptionService = batchTranscriptionService;
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();

            var request = body.Deserialize<SpeechRecognitionQueueRequest>();

            // Get blob stream
            var container = _blobServiceClient.GetBlobContainerClient("ogafiles");
            var blob = container.GetBlobClient(request.BlobGuid.ToString());
            var sasUri = blob.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow + TimeSpan.FromHours(48));

            await _batchTranscriptionService.CreateTranscription(
                new CreateTranscriptionRequest()
                {
                    ContentUrls = new List<string>
                    {
                        sasUri.ToString()
                    },
                    DisplayName = $"ChatId--{request.ConversationContext.Chat.Id}--MessageId--{request.ConversationContext.MessageId}--BlobGuid--{request.BlobGuid}",
                    Locale = "ru-RU",
                    Properties = new TranscriptionProperties()
                    {
                        DiarizationEnabled = false
                    }
                }, CancellationToken.None);

            await args.CompleteMessageAsync(args.Message);
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _processor.StartProcessingAsync(stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
            await _processor.StopProcessingAsync(stoppingToken);
        }
    }
}