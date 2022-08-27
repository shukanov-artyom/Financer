using Financer.SpeechRecognition.Models;

namespace Financer.SpeechRecognition.Services
{
    public interface IBatchTranscriptionService
    {
        Task CreateTranscription(CreateTranscriptionRequest createTranscriptionRequest, CancellationToken cancellationToken);

        Task<List<Transcription>> GetAllTranscriptions(CancellationToken cancellationtoken);

        Task<List<TranscriptionFile>> GetTranscriptionFiles(Transcription transcription, CancellationToken cancellationToken);

        Task DeleteTranscription(string id);
    }

    public class BatchTranscriptionService : IBatchTranscriptionService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BatchTranscriptionService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task CreateTranscription(CreateTranscriptionRequest createTranscriptionRequest, CancellationToken cancellationToken)
        {
            using var client = _httpClientFactory.CreateClient("TranscriptionsApi");
            await client.PostAsJsonAsync("", createTranscriptionRequest, cancellationToken);
        }

        public async Task DeleteTranscription(string id)
        {
            using var client = _httpClientFactory.CreateClient("TranscriptionsApi");
            await client.DeleteAsync($"transcriptions/{id}");
        }

        public async Task<List<Transcription>> GetAllTranscriptions(CancellationToken cancellationToken)
        {
            using var client = _httpClientFactory.CreateClient("TranscriptionsApi");
            var collection = await client.GetFromJsonAsync<TranscriptionsCollection>("", cancellationToken);
            return collection.Values;
        }

        public async Task<List<TranscriptionFile>> GetTranscriptionFiles(Transcription transcription, CancellationToken cancellationToken)
        {
            using var client = _httpClientFactory.CreateClient("TranscriptionsApi");
            var files = await client.GetFromJsonAsync<TranscriptionFileCollection>(transcription.Links.Files, cancellationToken);
            return files.Values;
        }
    }
}
