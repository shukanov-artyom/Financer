namespace Financer.Telegram.Inbound.Services.Models
{
    public class TelegramFileResponse
    {
        public bool Ok { get; set; }

        public TelegramFileResultResponse Result { get; set; }
    }

    public class TelegramFileResultResponse
    {
        public string FileId { get; set; }

        public string FileUniqueId { get; set; }

        public int FileSize { get; set; }

        public string FilePath { get; set; }
    }
}
