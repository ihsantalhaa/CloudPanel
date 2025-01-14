namespace CloudPanel.WebApp.Models.Dtos
{
    public class GetFileDto
    {
        public int fileId { get; set; }
        public string? fileName { get; set; }
        public string? fileDescription { get; set; }
        public string? filePath { get; set; }
    }
}
