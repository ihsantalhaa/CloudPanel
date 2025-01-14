namespace CloudPanel.WebApi.Dtos.GroupFileDtos
{
    public class GetGroupFilesByFileIdDto
    {
        public int FileId { get; set; }
        public int GroupId { get; set; }
        public string? GroupName { get; set; }
        public string? Description { get; set; }
    }
}
