using System;
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
        public async Task<SqlPagedResult<Users>> GetAllAsync(SqlPagedQuery<Users> sqlPagedQuery)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<Users>(sqlPagedQuery);
                var users = await sqlConnection.QueryAsync<Users>(sqlQueryBuilder.GetSelectQuery());
                var count = await sqlConnection.QueryFirstAsync<int>(sqlQueryBuilder.GetCountQuery());
                return SqlPagedResult<Users>.Create(users, sqlPagedQuery, count);
            }
        }
        public async Task<Users> GetUserAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<Users>()
                    .WithFilter(
                        new SqlQueryCondition
                        {
                            Name = "Id",
                            Operator = SqlQueryOperator.Equal,
                            Value = new string(id.ToString())
                        }
                    );
                var user = await sqlConnection.QueryFirstOrDefaultAsync<Users>(sqlQueryBuilder.GetSelectQuery());
                return user;
            }
        }
        public async Task<Users> GetUserAsync(string login)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<Users>()
                    .WithFilter(
                        new SqlQueryCondition
                        {
                            Name = "Login",
                            Operator = SqlQueryOperator.Equal,
                            Value = new string(login)
                        }
                    );
                var user = await sqlConnection.QueryFirstOrDefaultAsync<Users>(sqlQueryBuilder.GetSelectQuery());
                return user;
            }
        }
        public async Task<string> GetUserFullNameAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<Users>()
                    .WithFilter(
                        new SqlQueryCondition
                        {
                            Name = "Id",
                            Operator = SqlQueryOperator.Equal,
                            Value = new string(id.ToString())
                        }
                    );
                var user = await sqlConnection.QueryFirstOrDefaultAsync<Users>(sqlQueryBuilder.GetSelectQuery());
                return $"{user.FirstName} {user.LastName}";
            }
        }
        public async Task<string> GetUserDepartmentNameAsync(string login)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<Users>()
                    .WithFilter(
                        new SqlQueryCondition
                        {
                            Name = "Login",
                            Operator = SqlQueryOperator.Equal,
                            Value = new string(login)
                        }
                    );
                var user = await sqlConnection.QueryFirstOrDefaultAsync<Users>(sqlQueryBuilder.GetSelectQuery());
                return user.Department;
            }
        }
        public async Task<IEnumerable<string>> GetUsersAsync(string role)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<Users>()
                    .WithFilter(
                        new SqlQueryCondition
                        {
                            Name = "Role",
                            Operator = SqlQueryOperator.Equal,
                            Value = new string(role)
                        }
                    )
                    .WithSort(nameof(Users.Login), "ASC");
                var users = await sqlConnection.QueryAsync<string>(sqlQueryBuilder.GetSelectQuery());
                return users;
            }
        }
    }
}