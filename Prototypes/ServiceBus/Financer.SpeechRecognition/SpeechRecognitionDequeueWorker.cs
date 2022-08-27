using Azure.Storage.Blobs;
using Financer.Common;
using Financer.Common.Messaging;
using Financer.Common.Models;
using Financer.SpeechRecognition.Models;
using Financer.SpeechRecognition.Services;
using Telegram.Bot.Types;

namespace Financer.SpeechRecognition
{
    public class SpeechRecognitionDequeueWorker : BackgroundService
    {
        private readonly IBatchTranscriptionService _batchTranscriptionService;
        private readonly IServiceBusSender _sender;
        private readonly BlobServiceClient _blobServiceClient;

        public SpeechRecognitionDequeueWorker(
            IBatchTranscriptionService batchTranscriptionService,
            IServiceBusSender sender,
            BlobServiceClient blobServiceClient)
        {
            _batchTranscriptionService = batchTranscriptionService;
            _sender = sender;
            _blobServiceClient = blobServiceClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var transcriptions = await _batchTranscriptionService.GetAllTranscriptions(CancellationToken.None);

                foreach (var transcription in transcriptions.Where(t => t.Status == "Succeeded"))
                {
                    // $"ChatId--{request.ConversationContext.Chat.Id}--MessageId--{request.ConversationContext.MessageId}--BlobGuid--{request.BlobGuid}
                    var transcriptionName = transcription.DisplayName;
                    var split = transcriptionName.Split("--");
                    var chatId = Int32.Parse(split[1]);
                    var messageId = Int32.Parse(split[3]);
                    var file = await _batchTranscriptionService.GetTranscriptionFiles(transcription, CancellationToken.None);

                    foreach (var fileResult in file)
                    {
                        var client = new HttpClient();
                        var transcript = await client.GetFromJsonAsync<TranscriptionFileContent>(fileResult.Links.ContentUrl, CancellationToken.None);
                        var recognizedText = transcript?.CombinedRecognizedPhrases?.FirstOrDefault();
                        if (recognizedText != null)
                        {
                            var text = recognizedText.Lexical;
                            await _sender.Send(Queues.TelegramOutbound.Name,
                                new TelegramOutboundMessage
                                {
                                    MessageText = text,
                                    ConversationContext = new()
                                    {
                                        Chat = new Chat() { Id = chatId },
                                        MessageId = messageId
                                    }
                                });
                        }
                    }

                    var id = new Uri(transcription.Self).Segments.Last();
                    await _batchTranscriptionService.DeleteTranscription(id);

                    var containerClient = _blobServiceClient.GetBlobContainerClient("ogafiles");
                    var blobClient = containerClient.GetBlobClient(split[5]);
                    await blobClient.DeleteAsync();
                }

                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}