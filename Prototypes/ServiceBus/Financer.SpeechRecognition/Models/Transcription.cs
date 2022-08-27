namespace Financer.SpeechRecognition.Models
{
    public class Transcription
    {
        public string Self { get; set; }

        public TranscriptionModelLink Model { get; set; }

        public TranscriptionLinks Links { get; set; }

        public TranscriptionProperties Properties { get; set; }

        public DateTimeOffset LastActionDateTime { get; set; }

        public string Status { get; set; } // Suceeded

        public DateTimeOffset CreatedDateTime { get; set; }

        public string Locale { get; set; }

        public string DisplayName { get; set; }
    }

    public class TranscriptionModelLink
    {
        public string Self { get; set; }
    }
}
