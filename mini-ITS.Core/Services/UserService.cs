using System.Collections.Generic;
using System.Threading.Tasks;
using mini_ITS.Core.Repository;
using mini_ITS.Core.Models;
using mini_ITS.Core.Dto;
using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using mini_ITS.Core.Database;

namespace mini_ITS.Core.Services
{
    public class UserService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<Users> _hasher;
        private readonly IAuthorizationService _authorizationService;

        public UserService(IUsersRepository usersRepository, IMapper mapper, IPasswordHasher<Users> hasher, IAuthorizationService authorizationService)
        {
            _usersRepository = usersRepository;
            _mapper = mapper;
            _hasher = hasher;
            _authorizationService = authorizationService;
        }

        public async Task<IEnumerable<UserDto>> GetAsync()
        {
            var users = await _usersRepository.GetAsync();
            return users?.Select(y => _mapper.Map<UserDto>(y));
        }
        public async Task<SqlPagedResult<UserDto>> GetAsync(SqlPagedQuery<Users> query)
        {
            var result = await _usersRepository.GetAsync(query);
            var users = result.Results.Select(x => _mapper.Map<UserDto>(x));
            return users == null ? null : SqlPagedResult<UserDto>.From(result, users);
        }  
        public async Task<IEnumerable<UserDto>> GetAsync(string role, string department)
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
            return users?.Select(y => _mapper.Map<UserDto>(y));
        }
        public async Task<IEnumerable<UserList>> GetAllFullNameAsync(string role)
        {
            var userList = new List<UserList>();

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
                    userList.Add(new UserList(item.Id, $"{item.FirstName} {item.LastName}"));
                }
            }

            return userList ?? null;
        }
        public async Task<string> GetFullNameAsync(Guid id)
        {
            var user = await _usersRepository.GetAsync(id);
            return user == null ? null : $"{user.FirstName} {user.LastName}";
        }
        public async Task<UserDto> GetAsync(Guid id)
        {
            var user = await _usersRepository.GetAsync(id);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }
        public async Task<UserDto> GetAsync(string username)
        {
            var user = await _usersRepository.GetAsync(username);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }
        public async Task<string> GetDepartmentNameAsync(string username)
        {
            var user = await _usersRepository.GetAsync(username);
            return user?.Department;
        }


        public async Task CreateAsync(UserDto user)
        {
            var existingUser = await _usersRepository.GetAsync(user.Login);
            if (existingUser != null)
            {
                throw new Exception($"Użytkownik '{user.Login}' już istnieje w bazie.");
            }

            var newUser = new Users(Guid.NewGuid(), user.Login, user.FirstName, user.LastName, user.Department, user.Email, user.Phone, user.Role, "");
            newUser.PasswordHash = _hasher.HashPassword(newUser, user.Password);
            await _usersRepository.CreateAsync(newUser);
        }
        public async Task UpdateAsync(UserDto user)
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