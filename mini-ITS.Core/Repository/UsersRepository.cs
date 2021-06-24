using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using mini_ITS.Core.Database;
using mini_ITS.Core.Models;
using Dapper;

namespace mini_ITS.Core.Repository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly string _connectionString;

        public UsersRepository(ISqlConnectionString sqlConnectionString)
        {
            _connectionString = sqlConnectionString.ConnectionString;
        }

        public async Task<IEnumerable<Users>> GetAllAsync()
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<Users>()
                    .WithSort(nameof(Users.Login), "ASC");
                var users = await sqlConnection.QueryAsync<Users>(sqlQueryBuilder.GetSelectQuery());
                return users;
            }
        }
    }
}