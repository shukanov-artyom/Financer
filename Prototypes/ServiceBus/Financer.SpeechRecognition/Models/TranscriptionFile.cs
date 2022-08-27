namespace Financer.SpeechRecognition.Models
{
    public class TranscriptionFile
    {
        public string Self { get; set; }

        public string Name { get; set; }

        public string Kind { get; set; }

        public Dictionary<string, int> Properties { get; set; }

        public DateTimeOffset CreatedDateTime { get; set; }

        public TranscriptionFileLinks Links { get; set; }
    }

    public class TranscriptionFileCollection
    {
        public List<TranscriptionFile> Values { get; set; }
    }
}
