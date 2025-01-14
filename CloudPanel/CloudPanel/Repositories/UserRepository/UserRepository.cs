using CloudPanel.WebApi.Dtos.UserDtos;
using CloudPanel.WebApi.Models.DapperContext;
using Dapper;
using System.Data;
using BC = BCrypt.Net.BCrypt;

namespace CloudPanel.WebApi.Repositories.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly Context _context;
        private readonly IConfiguration _configuration;

        public UserRepository(Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task CreateUserAsync(CreateUserDto createUserDto)
        {
            string checkQuery = "select * from Users where Username=@username or Mail=@mail";
            var checkParameters = new DynamicParameters();
            checkParameters.Add("@username", createUserDto.Username);
            checkParameters.Add("@mail", createUserDto.Mail);

            string query = "Insert Into Users (Username, Password, Mail, Phone) values (@username, @password, @mail, @phone)";
            var parameters = new DynamicParameters();
            parameters.Add("@username", createUserDto.Username);
            parameters.Add("@password", BC.HashPassword(createUserDto.Password));
            parameters.Add("@mail", createUserDto.Mail);
            parameters.Add("@phone", createUserDto.Phone);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var existingUser = await connection.QueryFirstOrDefaultAsync(checkQuery, checkParameters);

                    if (existingUser == null)
                    {
                        var result = await connection.ExecuteAsync(query, parameters);
                        if (result == 0)
                        {
                            throw new Exception("User creation failed.");
                        }
                    }
                    else
                    {
                        if (existingUser.Username == createUserDto.Username && existingUser.Mail == createUserDto.Mail)
                            throw new Exception("Both username and email are already in use.");
                        else if (existingUser.Username == createUserDto.Username)
                            throw new Exception("Username is already in use.");
                        else if (existingUser.Mail == createUserDto.Mail)
                            throw new Exception("Email is already in use.");
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating user: {ex.Message}");
            }
        }

        public async Task DeleteUserAsync(int id)
        {
            string query = "Delete From Users Where UserId=@userId";
            var parameters = new DynamicParameters();
            parameters.Add("@userId", id);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteAsync(query, parameters);
                    if (result == 0)
                    {
                        throw new Exception("User deletion failed. User may not exist.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting user: {ex.Message}");
            }
        }

        public async Task<List<ResultUserDto>> GetAllUsersAsync()
        {
            string query = "Select * from Users";
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var values = await connection.QueryAsync<ResultUserDto>(query);
                    return values.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching users: {ex.Message}");
            }
        }

        public async Task<GetByIdUserDto> GetUserByIdAsync(int id)
        {
            string query = "Select * from Users Where UserId=@userId";
            var parameters = new DynamicParameters();
            parameters.Add("@userId", id);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var value = await connection.QueryFirstAsync<GetByIdUserDto>(query, parameters);
                    return value;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching users: {ex.Message}");
            }
        }

        public async Task UpdateUserAsync(UpdateUserDto updateUserDto)
        {
            string checkQuery = "select * from Users where (Username = @username or Mail = @mail) and UserId != @userId";
            var checkParameters = new DynamicParameters();
            checkParameters.Add("@userId", updateUserDto.UserId);
            checkParameters.Add("@username", updateUserDto.Username);
            checkParameters.Add("@mail", updateUserDto.Mail);

            string updateQuery = "Update Users Set Username=@username, Password=@password, Mail=@mail, Phone=@phone where UserId=@userId";
            var UpdateParameters = new DynamicParameters();
            UpdateParameters.Add("@userId", updateUserDto.UserId);
            UpdateParameters.Add("@username", updateUserDto.Username);
            UpdateParameters.Add("@password", BC.HashPassword(updateUserDto.Password));
            UpdateParameters.Add("@mail", updateUserDto.Mail);
            UpdateParameters.Add("@phone", updateUserDto.Phone);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var existingUser =  await connection.QueryFirstOrDefaultAsync(checkQuery, checkParameters);

                    if (existingUser == null)
                    {
                        var result = await connection.ExecuteAsync(updateQuery, UpdateParameters);
                        if (result == 0)
                        {
                            throw new Exception("User updating failed.");
                        }
                    }
                    else
                    {
                        if (existingUser.Username == updateUserDto.Username && existingUser.Mail == updateUserDto.Mail)
                            throw new Exception("Both username and email are already in use.");
                        else if (existingUser.Username == updateUserDto.Username)
                            throw new Exception("Username is already in use.");
                        else if (existingUser.Mail == updateUserDto.Mail)
                            throw new Exception("Email is already in use.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating user: {ex.Message}");
            }
        }
    }
}
