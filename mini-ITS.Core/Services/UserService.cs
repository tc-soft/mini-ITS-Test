using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using mini_ITS.Core.Database;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Repository;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<Users> _hasher;

        public UsersService(IUsersRepository usersRepository, IMapper mapper, IPasswordHasher<Users> hasher)
        {
            _usersRepository = usersRepository;
            _mapper = mapper;
            _hasher = hasher;
        }

        public async Task<IEnumerable<UsersDto>> GetAsync()
        {
            var users = await _usersRepository.GetAsync();
            return users?.Select(x => _mapper.Map<UsersDto>(x));
        }
        public async Task<IEnumerable<UsersDto>> GetAsync(List<SqlQueryCondition> sqlQueryConditionList)
        {
            var users = await _usersRepository.GetAsync(sqlQueryConditionList);
            return users?.Select(x => _mapper.Map<UsersDto>(x));
        }
        public async Task<SqlPagedResult<UsersDto>> GetAsync(SqlPagedQuery<Users> query)
        {
            var result = await _usersRepository.GetAsync(query);
            var users = result.Results.Select(x => _mapper.Map<UsersDto>(x));
            return users == null ? null : SqlPagedResult<UsersDto>.From(result, users);
        }  
        public async Task<IEnumerable<UsersDto>> GetAsync(string role, string department)
        {
            var filter = new List<SqlQueryCondition>()
            {
                new SqlQueryCondition
                {
                    Name = "Role",
                    Operator = SqlQueryOperator.Equal,
                    Value = new string(role)
                },
                new SqlQueryCondition
                {
                    Name = "Department",
                    Operator = SqlQueryOperator.Equal,
                    Value = new string(department)
                }
            };

            var users = await _usersRepository.GetAsync(filter);
            return users?.Select(x => _mapper.Map<UsersDto>(x));
        }
        public async Task<IEnumerable<UserListDto>> GetAllFullNameAsync(string role)
        {
            var userList = new List<UserListDto>();

            var filter = new List<SqlQueryCondition>()
            {
                new SqlQueryCondition
                {
                    Name = "Role",
                    Operator = SqlQueryOperator.Equal,
                    Value = new string(role)
                }
            };

            var users = await _usersRepository.GetAsync(filter);

            foreach (var item in users)
            {
                if(item.Login != "admin")
                {
                    userList.Add(new UserListDto(item.Id, $"{item.FirstName} {item.LastName}"));
                }
            }

            return userList ?? null;
        }
        public async Task<string> GetFullNameAsync(Guid id)
        {
            var user = await _usersRepository.GetAsync(id);
            return user == null ? null : $"{user.FirstName} {user.LastName}";
        }
        public async Task<UsersDto> GetAsync(Guid id)
        {
            var user = await _usersRepository.GetAsync(id);
            return user == null ? null : _mapper.Map<UsersDto>(user);
        }
        public async Task<UsersDto> GetAsync(string username)
        {
            var user = await _usersRepository.GetAsync(username);
            return user == null ? null : _mapper.Map<UsersDto>(user);
        }
        public async Task<string> GetDepartmentNameAsync(string username)
        {
            var user = await _usersRepository.GetAsync(username);
            return user?.Department;
        }

        public async Task CreateAsync(UsersDto user)
        {
            var existingUser = await _usersRepository.GetAsync(user.Login);
            if (existingUser != null)
            {
                throw new Exception($"Użytkownik '{user.Login}' już istnieje w bazie.");
            }

            var newUser = new Users(Guid.NewGuid(), user.Login, user.FirstName, user.LastName, user.Department, user.Email, user.Phone, user.Role, "");
            newUser.PasswordHash = _hasher.HashPassword(newUser, user.PasswordHash);
            await _usersRepository.CreateAsync(newUser);
        }
        public async Task UpdateAsync(UsersDto user)
        {
            var updateUser = await _usersRepository.GetAsync(user.Id);
            var tempUser = await _usersRepository.GetAsync(user.Login);

            if(tempUser != null)
            {
                if(updateUser.Id == tempUser.Id)
                {
                    //Nie nastąpiła zmiana Loginu
                    updateUser.Login = user.Login;
                    updateUser.FirstName = user.FirstName;
                    updateUser.LastName = user.LastName;
                    updateUser.Department = user.Department;
                    updateUser.Email = user.Email;
                    updateUser.Phone = user.Phone;
                    updateUser.Role = user.Role;

                    await _usersRepository.UpdateAsync(updateUser);
                }
                else
                {
                    throw new Exception($"Użytkownik '{user.Login}' już istnieje w bazie.");
                }
            }
            else
            {
                //tempUser == null - zmieniony Login nie jest zajety
                updateUser.Login = user.Login;
                updateUser.FirstName = user.FirstName;
                updateUser.LastName = user.LastName;
                updateUser.Department = user.Department;
                updateUser.Email = user.Email;
                updateUser.Phone = user.Phone;
                updateUser.Role = user.Role;

                await _usersRepository.UpdateAsync(updateUser);
            }
        }
        public async Task DeleteAsync(Guid id)
        {
            await _usersRepository.DeleteAsync(id);
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            bool results = false;

            var user = await _usersRepository.GetAsync(username);
            if (user is not null)
            {
                var passwordVerification = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
                if (passwordVerification == PasswordVerificationResult.Success)
                {
                    results = true;
                }
            }
            else
            {
                throw new Exception($"Brak użytkownika '{username}' w bazie.");
            }

            return results;
        }
        public async Task ChangePasswordAsync(string Login, string oldPassword, string newPassword)
        {
            var user = await _usersRepository.GetAsync(Login);
            var passwordVerification = _hasher.VerifyHashedPassword(user, user.PasswordHash, oldPassword);
            if (passwordVerification == PasswordVerificationResult.Failed)
            {
                throw new Exception("Niepoprawne stare hasło.");
            }
            await _usersRepository.SetPasswordAsync(user.Id, _hasher.HashPassword(user, newPassword));
        }
        public async Task SetPasswordAsync(string Login, string password)
        {
            var user = await _usersRepository.GetAsync(Login);

            if (user is not null)
            {
                await _usersRepository.SetPasswordAsync(user.Id, _hasher.HashPassword(user, password));
            }
            else
            {
                throw new Exception($"Użytkownik '{user.Login}' nie istnieje w bazie.");
            }
        }
    }
}