namespace CloudPanel.WebApp.Models.ViewModels
{
    public class LogStreamVM
    {
        public string? arn { get; set; }
        public string? creationTime { get; set; }
        public string? firstEventTimestamp { get; set; }
        public string? lastEventTimestamp { get; set; }
        public string? lastIngestionTime { get; set; }
        public string? logStreamName { get; set; }
        public int storedBytes { get; set; }
        public string? uploadSequenceToken { get; set; }
    }
}
