namespace CloudPanel.WebApi.Dtos.UserDtos
{
    public class UpdateUserDto
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Mail { get; set; }
        public string? Phone { get; set; }
    }
}
