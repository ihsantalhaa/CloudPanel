using CloudPanel.WebApi.Dtos.GroupFileDtos;
using CloudPanel.WebApi.Dtos.GroupUserDtos;
using CloudPanel.WebApi.Models.DapperContext;
using Dapper;

namespace CloudPanel.WebApi.Repositories.GroupFileRepository
{
    public class GroupFileRepository : IGroupFileRepository
    {
        private readonly Context _context;

        public GroupFileRepository(Context context)
        {
            _context = context;
        }

        public async Task CreateGroupFileAsync(CreateGroupFileDto createGroupFileDto)
        {
            string query = "Insert Into GroupFiles (FileId, GroupId) values (@fileId, @groupId)";
            var parameters = new DynamicParameters();
            parameters.Add("@fileId", createGroupFileDto.FileId);
            parameters.Add("@groupId", createGroupFileDto.GroupId);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteAsync(query, parameters);
                    if (result == 0)
                    {
                        throw new Exception("GroupFile creation failed.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating GroupFile: {ex.Message}");
            }
        }

        public async Task DeleteGroupFileAsync(int fileId, int groupId)
        {
            string query = "Delete From GroupFiles Where FileId=@fileId and GroupId=@groupId";
            var parameters = new DynamicParameters();
            parameters.Add("@fileId", fileId);
            parameters.Add("@groupId", groupId);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteAsync(query, parameters);
                    if (result == 0)
                    {
                        throw new Exception("GroupFile deletion failed. GroupFile may not exist.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting GroupFile: {ex.Message}");
            }
        }

        public async Task<List<GetGroupFilesAndUsersDto>> GetGroupFilesAndUsersAsync(int groupId)
        {

            string query = "Select GroupFiles.FileId, GroupFiles.GroupId, Files.FileName, Files.FileDescription, Files.FilePath, Files.UserId, Users.Username" +
              " from GroupFiles inner join Files on Files.FileId = GroupFiles.FileId inner join Users on Users.UserId = Files.UserId Where GroupFiles.GroupId=@groupId";
            var parameters = new DynamicParameters();
            parameters.Add("@groupId", groupId);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var values = await connection.QueryAsync<GetGroupFilesAndUsersDto>(query, parameters);
                    return values.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching GroupFiles: {ex.Message}");
            }
        }

        public async Task<List<GetGroupFilesByFileIdDto>> GetGroupFilesByFileIdAsync(int fileId)
        {
            string query = "Select GroupFiles.FileId, GroupFiles.GroupId, Groups.GroupName, Groups.Description from GroupFiles inner join Groups on GroupFiles.GroupId = Groups.GroupId Where GroupFiles.FileId=@fileId";
            var parameters = new DynamicParameters();
            parameters.Add("@fileId", fileId);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var values = await connection.QueryAsync<GetGroupFilesByFileIdDto>(query, parameters);
                    return values.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching groups: {ex.Message}");
            }
        }
    }
}
