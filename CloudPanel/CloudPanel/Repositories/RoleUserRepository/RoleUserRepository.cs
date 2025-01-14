using CloudPanel.WebApi.Dtos.RoleUserDtos;
using CloudPanel.WebApi.Models.DapperContext;
using Dapper;

namespace CloudPanel.WebApi.Repositories.RoleUserRepository
{
    public class RoleUserRepository : IRoleUserRepository
    {
        private readonly Context _context;

        public RoleUserRepository(Context context)
        {
            _context = context;
        }

        public async Task CreateRoleUserAsync(CreateRoleUserDto createRoleUserDto)
        {
            string query = "Insert Into RoleUsers (UserId, RoleId) values (@userId, @roleId)";
            var parameters = new DynamicParameters();
            parameters.Add("@userId", createRoleUserDto.UserId);
            parameters.Add("@roleId", createRoleUserDto.RoleId);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteAsync(query, parameters);
                    if (result == 0)
                    {
                        throw new Exception("RoleUser creation failed.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating RoleUser: {ex.Message}");
            }
        }

        public async Task DeleteRoleUserAsync(int userId, int roleId)
        {
            string query = "Delete From RoleUsers Where UserId=@userId and RoleId=@roleId";
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId);
            parameters.Add("@roleId", roleId);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteAsync(query, parameters);
                    if (result == 0)
                    {
                        throw new Exception("RoleUser deletion failed. RoleUser may not exist.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting RoleUser: {ex.Message}");
            }
        }

        public async Task<List<GetRoleUserByRoleIdDto>> GetRoleUserByRoleIdAsync(int roleId)
        {
            string query = "SELECT RoleUsers.RoleId, RoleUsers.UserId, Users.Username FROM RoleUsers INNER JOIN Users ON RoleUsers.UserId = Users.UserId WHERE RoleUsers.RoleId = @roleId";
            var parameters = new DynamicParameters();
            parameters.Add("@roleId", roleId);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var values = await connection.QueryAsync<GetRoleUserByRoleIdDto>(query, parameters);
                    return values.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching users: {ex.Message}");
            }
        }

        public async Task<List<GetRoleUserByUserIdDto>> GetRoleUserByUserIdAsync(int userId)
        {
            string query = "Select RoleUsers.UserId, RoleUsers.RoleId, Roles.RoleName from RoleUsers inner join Roles on RoleUsers.RoleId = Roles.RoleId Where RoleUsers.UserId=@userId";
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var values = await connection.QueryAsync<GetRoleUserByUserIdDto>(query, parameters);
                    return values.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching roles: {ex.Message}");
            }
        }


    }
}
