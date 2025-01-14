using CloudPanel.WebApi.Dtos.GroupUserDtos;
using CloudPanel.WebApi.Dtos.RoleUserDtos;
using CloudPanel.WebApi.Models.DapperContext;
using Dapper;

namespace CloudPanel.WebApi.Repositories.GroupUserRepository
{
    public class GroupUserRepository(Context context) : IGroupUserRepository
    {
        private readonly Context _context = context;

        public async Task CreateGroupUserAsync(CreateGroupUserDto createGroupUserDto)
        {
            string query = "Insert Into GroupUsers (UserId, GroupId) values (@userId, @groupId)";
            var parameters = new DynamicParameters();
            parameters.Add("@userId", createGroupUserDto.UserId);
            parameters.Add("@groupId", createGroupUserDto.GroupId);
            try
            {
                using var connection = _context.CreateConnection();
                var result = await connection.ExecuteAsync(query, parameters);
                if (result == 0)
                {
                    throw new Exception("GroupUser creation failed.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating GroupUser: {ex.Message}");
            }
        }

        public async Task DeleteGroupUserAsync(int userId, int groupId)
        {
            string query = "Delete From GroupUsers Where UserId=@userId and GroupId=@groupId";
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId);
            parameters.Add("@groupId", groupId);
            try
            {
                using var connection = _context.CreateConnection();
                var result = await connection.ExecuteAsync(query, parameters);
                if (result == 0)
                {
                    throw new Exception("GroupUser deletion failed. RoleUser may not exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting GroupUser: {ex.Message}");
            }
        }

        public async Task<List<GetGroupUserByGroupIdDto>> GetGroupUserByGroupIdAsync(int groupId)
        {
            string query = "SELECT GroupUsers.GroupId, GroupUsers.UserId, Users.Username FROM GroupUsers INNER JOIN Users ON GroupUsers.UserId = Users.UserId WHERE GroupUsers.GroupId = @groupId";
            var parameters = new DynamicParameters();
            parameters.Add("@groupId", groupId);
            try
            {
                using var connection = _context.CreateConnection();
                var values = await connection.QueryAsync<GetGroupUserByGroupIdDto>(query, parameters);
                return values.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching users: {ex.Message}");
            }
        }

        public async Task<List<GetGroupUserByUserIdDto>> GetGroupUserByUserIdAsync(int userId)
        {
            string query = "Select GroupUsers.UserId, GroupUsers.GroupId, Groups.GroupName from GroupUsers inner join Groups on GroupUsers.GroupId = Groups.GroupId Where GroupUsers.UserId=@userId";
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId);
            try
            {
                using var connection = _context.CreateConnection();
                var values = await connection.QueryAsync<GetGroupUserByUserIdDto>(query, parameters);
                return values.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching roles: {ex.Message}");
            }
        }
    }
}
