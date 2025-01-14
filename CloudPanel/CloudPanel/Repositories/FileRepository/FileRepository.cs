using CloudPanel.WebApi.Dtos.FileDtos;

using CloudPanel.WebApi.Models.DapperContext;
using Dapper;

namespace CloudPanel.WebApi.Repositories.FileRepository
{
    public class FileRepository : IFileRepository
    {
        private readonly Context _context;
        public FileRepository(Context context)
        {
            _context = context;
        }

        public async Task CreateFileAsync(CreateFileDto createFileDto)
        {
            string query = "Insert Into Files (FileName, FileDescription, FilePath, UserId) values (@fileName, @fileDescription, @filePath, @userId)";
            var parameters = new DynamicParameters();
            parameters.Add("@fileName", createFileDto.FileName);
            parameters.Add("@fileDescription", createFileDto.FileDescription);
            parameters.Add("@filePath", createFileDto.FilePath);
            parameters.Add("@userId", createFileDto.UserId);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteAsync(query, parameters);
                    if (result == 0)
                    {
                        throw new Exception("File creation failed.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating file: {ex.Message}");
            }
        }

        public async Task UpdateFileAsync(UpdateFileDto updateFileDto)
        {
            string query = "Update Files Set FileName=@fileName, FileDescription=@fileDescription, FilePath=@filepath where FileId=@fileId";
            var parameters = new DynamicParameters();
            parameters.Add("@fileName", updateFileDto.FileName);
            parameters.Add("@fileDescription", updateFileDto.FileDescription);
            parameters.Add("@filePath", updateFileDto.FilePath);
            parameters.Add("@fileId", updateFileDto.FileId);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteAsync(query, parameters);
                    if (result == 0)
                    {
                        throw new Exception("File update failed. File may not exist.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating File: {ex.Message}");
            }
        }

        public async Task DeleteFileAsync(int fileId)
        {
            string query = "Delete From Files Where FileId=@fileId";
            var parameters = new DynamicParameters();
            parameters.Add("@fileId", fileId);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteAsync(query, parameters);
                    if (result == 0)
                    {
                        throw new Exception("File deletion failed. File may not exist.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting file: {ex.Message}");
            }
        }

        public async Task<List<ResultFileDto>> GetAllFilesAsync()
        {
            string query = "Select * from Files";
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var values = await connection.QueryAsync<ResultFileDto>(query);
                    return values.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching files: {ex.Message}");
            }
        }

        public async Task<GetByIdFileDto> GetFileByIdAsync(int fileId)
        {
            string query = "Select * from Files Where FileId=@fileId";
            var parameters = new DynamicParameters();
            parameters.Add("@fileId", fileId);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var value = await connection.QueryFirstAsync<GetByIdFileDto>(query, parameters);
                    return value;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching files: {ex.Message}");
            }
        }

        public async Task<List<GetUserFilesDto>> GetUserFilesByIdAsync(int userId)
        {
            string query = "Select * from Files Where UserId=@userId";
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var values = await connection.QueryAsync<GetUserFilesDto>(query, parameters);
                    return values.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching user files: {ex.Message}");
            }
        }
    }
}
