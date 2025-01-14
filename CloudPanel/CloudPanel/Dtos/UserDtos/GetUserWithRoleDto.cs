namespace CloudPanel.WebApi.Dtos.UserDtos
{
    public class GetUserWithRoleDto
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Mail { get; set; }
        public string? Phone { get; set; }

        public int RoleId { get; set; }
        public string? RoleName { get; set; }
    }
}
