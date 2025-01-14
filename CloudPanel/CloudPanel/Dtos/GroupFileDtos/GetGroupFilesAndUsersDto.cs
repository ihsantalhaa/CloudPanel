namespace CloudPanel.WebApi.Dtos.GroupFileDtos
{
    public class GetGroupFilesAndUsersDto
    {
        public int GroupId { get; set; }
        public int FileId { get; set; }
        public string? FileName { get; set; }
        public string? FileDescription { get; set; }
        public string? FilePath { get; set; }
        public int UserId { get; set; }
        public string? Username { get; set; }
    }
}
