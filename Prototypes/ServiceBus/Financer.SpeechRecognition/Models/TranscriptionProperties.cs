namespace Financer.SpeechRecognition.Models
{
    public class TranscriptionProperties
    {
        public bool DiarizationEnabled { get; set; }

        public bool WordLevelTimestampsEnabled { get; set; }

        public List<int> Channels { get; set; }

        public string PunctuationMode { get; set; }

        public string ProfanityFilterMode { get; set; }

        public string Duration { get; set; }
    }
}
