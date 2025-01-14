using CloudPanel.WebApi.Dtos.GroupDtos;
using CloudPanel.WebApi.Models.DapperContext;
using Dapper;

namespace CloudPanel.WebApi.Repositories.GroupRepository
{
    public class GroupRepository : IGroupRepository
    {
        private readonly Context _context;

        public GroupRepository(Context context)
        {
            _context = context;
        }

        public async Task CreateGroupAsync(CreateGroupDto createGroupDto)
        {
            string query = "Insert Into Groups (GroupName, Description) values (@groupName, @description)";
            var parameters = new DynamicParameters();
            parameters.Add("@groupName", createGroupDto.GroupName);
            parameters.Add("@description", createGroupDto.Description);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteAsync(query, parameters);
                    if (result == 0)
                    {
                        throw new Exception("Group creation failed.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating group: {ex.Message}");
            }
        }

        public async Task UpdateGroupAsync(UpdateGroupDto updateGroupDto)
        {
            string query = "Update Groups Set GroupName=@groupName, Description=@description where GroupId=@groupId";
            var parameters = new DynamicParameters();
            parameters.Add("@groupName", updateGroupDto.GroupName);
            parameters.Add("@description", updateGroupDto.Description);
            parameters.Add("@groupId", updateGroupDto.GroupId);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteAsync(query, parameters);
                    if (result == 0)
                    {
                        throw new Exception("Group update failed. Group may not exist.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating group: {ex.Message}");
            }
        }

        public async Task DeleteGroupAsync(int id)
        {
            string query = "Delete From Groups Where GroupId=@groupId";
            var parameters = new DynamicParameters();
            parameters.Add("@groupId", id);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteAsync(query, parameters);
                    if (result == 0)
                    {
                        throw new Exception("Group deletion failed. Group may not exist.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting group: {ex.Message}");
            }
        }

        public async Task<List<ResultGroupDto>> GetAllGroupsAsync()
        {
            string query = "Select * from Groups";
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var values = await connection.QueryAsync<ResultGroupDto>(query);
                    return values.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching groups: {ex.Message}");
            }
        }

        public async Task<GetByIdGroupDto> GetGroupByIdAsync(int id)
        {
            string query = "Select * from Groups Where GroupId=@groupId";
            var parameters = new DynamicParameters();
            parameters.Add("@groupId", id);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var value = await connection.QueryFirstAsync<GetByIdGroupDto>(query, parameters);
                    return value;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching groups: {ex.Message}");
            }
        }
    }
}
