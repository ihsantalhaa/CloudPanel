namespace CloudPanel.WebApi.Dtos.FileDtos
{
    public class CreateFileDto
    {
        public string? FileName { get; set; }
        public string? FileDescription { get; set; }
        public string? FilePath { get; set; }
        public int UserId { get; set; }
    }
}
