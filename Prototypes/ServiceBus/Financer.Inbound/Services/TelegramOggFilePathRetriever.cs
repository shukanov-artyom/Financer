using Financer.Common.Serialization;
using Financer.Telegram.Inbound.Services.Models;

namespace Financer.Inbound.Services
{
    public interface IVoiceFilePathRetriever
    {
        Task<string> GetAudioFilePath(string fileId);
    }

    public class TelegramOggFilePathRetriever : IVoiceFilePathRetriever
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public TelegramOggFilePathRetriever(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetAudioFilePath(string fileId)
        {
            if (string.IsNullOrEmpty(fileId))
                throw new ArgumentNullException(nameof(fileId));

            var httpClient = _httpClientFactory.CreateClient("TelegramBotApi");
            var responseMessage = await httpClient.GetAsync($"getFile?file_id={fileId}");
            if (responseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await responseMessage.Content.ReadAsStreamAsync();
                var fileInfo = await contentStream.DeserializeAsync<TelegramFileResponse>();
                if (fileInfo.Ok)
                {
                    var fileClient = _httpClientFactory.CreateClient("TelegramFileApi");
                    return $"{fileClient.BaseAddress}{fileInfo.Result.FilePath}";
                }
            }
            return string.Empty;
        }
    }
}
