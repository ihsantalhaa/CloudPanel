namespace CloudPanel.WebApi.Dtos.GroupUserDtos
{
    public class GetGroupUserByUserIdDto
    {
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public string? GroupName { get; set; }
    }
}
