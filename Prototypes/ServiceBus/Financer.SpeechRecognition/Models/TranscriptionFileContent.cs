namespace Financer.SpeechRecognition.Models
{
    public class TranscriptionFileContent
    {
        public string Source { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public long DurationInTicks { get; set; }

        public string Duration { get; set; }

        public List<CombinedRecognizedPhrases> CombinedRecognizedPhrases { get; set; }

        //public string RecognizedPhrases { get; set; }
    }

    public class CombinedRecognizedPhrases
    {
        public int Channel { get; set; }

        public string Lexical { get; set; }

        public string Itn { get; set; }

        public string MaskedITN { get; set; }

        public string Display { get; set; }
    }
}
