namespace CloudPanel.WebApp.Models.ViewModels
{
    public class UserFileVM
    {
        public int userId { get; set; }
        public int fileId { get; set; }
        public string? fileName { get; set; }
        public string? fileDescription { get; set; }
        public string? filePath { get; set; }
    }
}
