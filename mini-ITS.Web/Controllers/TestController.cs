using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using mini_ITS.Core;
using mini_ITS.Core.Repository;
using mini_ITS.Core.Models;
using mini_ITS.Core.Database;

namespace mini_ITS.Web.Controllers
{
    public class TestController : Controller
    {
        private readonly IUsersRepository _usersRepository;

        public TestController(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var myFilter = new List<SqlQueryCondition>()
            {
                new SqlQueryCondition
                {
                    Name = "Login",
                    Operator = SqlQueryOperator.LIKE,
                    Value = new string("%cisz%")
                },
                new SqlQueryCondition
                {
                    Name = "Login",
                    Operator = SqlQueryOperator.Equal,
                    Value = new string("ciszetad")
                }
            };

            var result = await _usersRepository.GetAsync(new SqlPagedQuery<Users>
            {
                //Filter = myFilter,
                SortColumnName = "Login",
                SortDirection = "ASC",
                Page = 1,
                Results = 20
            });

            Guid guid = new Guid("e5daa03f-8dfa-4d1a-87b1-22d971f9654c");

            var user = new Users
            {
                Id = guid,
                Login = "ciszetad",
                FirstName = "Tadeuszo",
                LastName = "Ciszewskio",
                Department = "IT",
                Email = "office@tc-soft.pl",
                Phone = "502600121",
                Role = "Kierownik",
                PasswordHash = "dstgxstgv"
            };

            //await _userRepository.CreateUserAsync(user);
            //var result = await _userRepository.GetUserAsync(guid);

            return Ok(result);
        }
    }
}
