using MySql.Data.MySqlClient;
using System.Data;

namespace CloudPanel.WebApi.Models.DapperContext
{
    public class Context
    {
        private readonly IConfiguration _configuration;
        private readonly String _connectionString;

        public Context(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public IDbConnection CreateConnection() => new MySqlConnection(_connectionString);
        
    }
}
