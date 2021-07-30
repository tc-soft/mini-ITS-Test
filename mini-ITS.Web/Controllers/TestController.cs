using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mini_ITS.Core.Services;
using mini_ITS.Core.Repository;
using mini_ITS.Core.Models;
using mini_ITS.Core.Database;

namespace mini_ITS.Web.Controllers
{
    //[Produces("application/json")]
    public class TestController : Controller
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IUsersService _usersService;

        public TestController(IUsersRepository usersRepository, IUsersService usersService)
        {
            _usersRepository = usersRepository;
            _usersService = usersService;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            var myFilter = new List<SqlQueryCondition>()
            {
                new SqlQueryCondition
                {
                    Name = "Role",
                    Operator = SqlQueryOperator.Equal,
                    Value = null
                    //Value = new string("")
                },
                new SqlQueryCondition
                {
                    Name = "Department",
                    Operator = SqlQueryOperator.Equal,
                    Value = null
                    //Value = new string("Managers")
                }
            };

            //var result = await _usersRepository.GetAsync(myFilter);

            var result = await _usersService.GetAsync(new SqlPagedQuery<Users>
            {
                Filter = myFilter,
                SortColumnName = "Login",
                SortDirection = "ASC",
                Page = 1,
                ResultsPerPage = 20
            });

            Guid guid = new Guid("e5daa03f-8dfa-4d1a-87b1-22d971f9654c");

            var user = new Users
            {
                Id = guid,
                Login = "ciszetad",
                FirstName = "Tadeuszo",
                LastName = "Ciszewskim",
                Department = "IT",
                Email = "office@tc-soft.pl",
                Phone = "502600121",
                Role = "Kierownik",
                PasswordHash = "dstgxstgvpir"
            };

            //await _usersService.SetPasswordAsync("atkincol", "NoweHgbvufnvuasło123#");

            //await _usersService.SetPasswordAsync("ciszetad", "portki200$");
            //await _usersRepository.DeleteAsync(guid);
            //var result = await _usersService.GetAsync();

            return Ok(result);
        }
    }
}
