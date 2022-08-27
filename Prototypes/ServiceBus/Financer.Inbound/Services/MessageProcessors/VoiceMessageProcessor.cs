using Azure.Storage.Blobs;
using Financer.Common;
using Financer.Common.Messaging;
using Financer.Common.Models;
using Financer.Inbound.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Financer.Telegram.Inbound.Services.MessageProcessors
{
    public class VoiceMessageProcessor : ITelegramMessageProcessor
    {
        private readonly IVoiceFilePathRetriever _voiceFilePathRetriever;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IServiceBusSender _serviceBusSender;

        public VoiceMessageProcessor(IVoiceFilePathRetriever voiceFilePathRetriever,
            BlobServiceClient blobServiceClient,
            IServiceBusSender serviceBusSender)
        {
            _voiceFilePathRetriever = voiceFilePathRetriever;
            _blobServiceClient = blobServiceClient;
            _serviceBusSender = serviceBusSender;
        }

        public MessageType MessageType => MessageType.Voice;

        public async Task Process(Message message, CancellationToken cancellationToken = default)
        {
            var fileId = message?.Voice?.FileId;
            var filePath = await _voiceFilePathRetriever.GetAudioFilePath(fileId);

            // Upload file to blob storage
            using var client = new HttpClient();
            using var response = await client.GetAsync(filePath);
            using var streamToReadFrom = 
                await response.Content.ReadAsStreamAsync(cancellationToken);

            var blobContainerClient = _blobServiceClient.GetBlobContainerClient("ogafiles");

            var blobGuid = Guid.NewGuid();
            var blobClient = blobContainerClient.GetBlobClient(blobGuid.ToString());
            await blobClient.UploadAsync(streamToReadFrom, true);

            // Send queue message with a link to blob
            await _serviceBusSender.Send(Queues.SpeechRecognitionInput.Name,
                new SpeechRecognitionQueueRequest()
                {
                    BlobGuid = blobGuid,
                    ConversationContext = new()
                    {
                        Chat = message.Chat,
                        MessageId = message.MessageId
                    }
                });
        }
    }
}
