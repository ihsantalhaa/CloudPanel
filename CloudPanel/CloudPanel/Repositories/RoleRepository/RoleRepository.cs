using CloudPanel.WebApi.Dtos.GroupDtos;
using CloudPanel.WebApi.Dtos.RoleDtos;
using CloudPanel.WebApi.Dtos.UserDtos;
using CloudPanel.WebApi.Models.DapperContext;
using Dapper;

namespace CloudPanel.WebApi.Repositories.RoleRepository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly Context _context;
        public RoleRepository(Context context)
        {
            _context = context;
        }

        public async Task CreateRoleAsync(CreateRoleDto createRoleDto)
        {
            string query = "Insert Into Roles (RoleName) values (@roleName)";
            var parameters = new DynamicParameters();
            parameters.Add("@roleName", createRoleDto.RoleName);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteAsync(query, parameters);
                    if (result == 0)
                    {
                        throw new Exception("Role creation failed.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating role: {ex.Message}");
            }
        }

        public async Task DeleteRoleAsync(int id)
        {
            string query = "Delete From Roles Where RoleId=@roleId";
            var parameters = new DynamicParameters();
            parameters.Add("@roleId", id);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteAsync(query, parameters);
                    if (result == 0)
                    {
                        throw new Exception("Role deletion failed. Role may not exist.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting role: {ex.Message}");
            }
        }

        public async Task<List<ResultRoleDto>> GetAllRolesAsync()
        {
            string query = "Select * from Roles";
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var values = await connection.QueryAsync<ResultRoleDto>(query);
                    return values.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching roles: {ex.Message}");
            }
        }

        public async Task<GetByIdRoleDto> GetRoleByIdAsync(int id)
        {
            string query = "Select * from Roles Where roleId=@roleId";
            var parameters = new DynamicParameters();
            parameters.Add("@roleId", id);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var value = await connection.QueryFirstAsync<GetByIdRoleDto>(query, parameters);
                    return value;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching roles: {ex.Message}");
            }
        }

        public async Task UpdateRoleAsync(UpdateRoleDto updateRoleDto)
        {
            string query = "Update Roles Set RoleName=@roleName where RoleId=@roleId";
            var parameters = new DynamicParameters();
            parameters.Add("@roleId", updateRoleDto.RoleId);
            parameters.Add("@roleName", updateRoleDto.RoleName);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteAsync(query, parameters);
                    if (result == 0)
                    {
                        throw new Exception("Role update failed. Role may not exist.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating role: {ex.Message}");
            }
        }
    }
}
