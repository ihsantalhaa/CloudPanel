namespace CloudPanel.WebApp.Models.Dtos
{
    public class CreateFileDto
    {
        public string? fileName { get; set; }
        public string? fileDescription { get; set; }
        public string? filePath { get; set; }
        public int userId { get; set; }
    }
}
