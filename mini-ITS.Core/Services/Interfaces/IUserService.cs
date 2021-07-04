using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mini_ITS.Core.Database;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Services
{
    public interface IUsersService
    {
        Task<IEnumerable<UsersDto>> GetAsync();
        Task<IEnumerable<UsersDto>> GetAsync(string role, string department);
        Task<IEnumerable<UsersDto>> GetAsync(List<SqlQueryCondition> sqlQueryConditionList);
        Task<SqlPagedResult<UsersDto>> GetAsync(SqlPagedQuery<Users> query);
        Task<UsersDto> GetAsync(Guid id);
        Task<UsersDto> GetAsync(string username);

        Task<string> GetFullNameAsync(Guid id);
        Task<string> GetDepartmentNameAsync(string username);
        Task<IEnumerable<UserListDto>> GetAllFullNameAsync(string role);

        Task CreateAsync(UsersDto user);
        Task UpdateAsync(UsersDto user);
        Task DeleteAsync(Guid id);

        Task<bool> LoginAsync(string username, string password);
        Task ChangePasswordAsync(string Login, string oldPassword, string newPassword);
        Task SetPasswordAsync(string Login, string password);
    }
}