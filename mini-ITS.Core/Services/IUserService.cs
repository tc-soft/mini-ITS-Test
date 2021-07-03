using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using mini_ITS.Core.Database;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Services
{
    public interface IUsersService
    {
        Task<IEnumerable<UserDto>> GetAsync();
        Task<SqlPagedResult<UserDto>> GetAsync(SqlPagedQuery<Users> query);
        Task<IEnumerable<UserDto>> GetAsync(string role, string department);
        Task<IEnumerable<UserList>> GetAllFullNameAsync(string role);
        Task<string> GetFullNameAsync(Guid id);
        Task<UserDto> GetAsync(Guid id);
        Task<UserDto> GetAsync(string username);
        Task<string> GetDepartmentNameAsync(string username);

        Task CreateAsync(UserDto user);
        Task UpdateAsync(UserDto user);
        Task DeleteAsync(Guid id);

        Task<bool> LoginAsync(string username, string password);
        Task ChangePasswordAsync(string Login, string oldPassword, string newPassword);
        Task SetPasswordAsync(string Login, string password);
    }
}