namespace CloudPanel.WebApp.Models
{
    public class S3File
    {
        public int fileId { get; set; }
        public string? fileName { get; set; }
        public string? fileDescription { get; set; }
        public string? filePath { get; set; }
        public int userId { get; set; }
    }
}
