namespace CloudPanel.WebApi.Dtos.RoleUserDtos
{
    public class GetRoleUserByUserIdDto
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
    }
}
