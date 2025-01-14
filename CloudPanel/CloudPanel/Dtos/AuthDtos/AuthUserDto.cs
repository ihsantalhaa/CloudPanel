namespace CloudPanel.WebApi.Dtos.LoginDtos
{
    public class AuthUserDto
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Mail { get; set; }
        public string? Phone { get; set; }
        public string? Password { get; set; }
    }
}
