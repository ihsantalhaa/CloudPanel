namespace CloudPanel.WebApi.Dtos.GroupUserDtos
{
    public class GetGroupUserByGroupIdDto
    {
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public string? Username { get; set; }
    }
}
