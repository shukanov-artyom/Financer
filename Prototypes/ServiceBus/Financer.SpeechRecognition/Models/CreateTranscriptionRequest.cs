namespace Financer.SpeechRecognition.Models
{
    public class CreateTranscriptionRequest
    {
        public List<string> ContentUrls { get; set; }

        public TranscriptionProperties Properties { get; set; }

        public string Locale { get; set; }

        public string DisplayName { get; set; }
    }
}
