using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mini_ITS.Core.Database;
using mini_ITS.Core.Models;
using mini_ITS.Core.Services;
using mini_ITS.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace mini_ITS.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUsersService _usersServices;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsersController(IUsersService usersServices, IHttpContextAccessor httpContextAccessor)
        {
            _usersServices = usersServices;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync([FromBody] LoginData loginData)
        {
            try
            {
                if (await _usersServices.LoginAsync(loginData.Login, loginData.Password))
                {
                    var usersDto = await _usersServices.GetAsync(loginData.Login);
                    var claimList = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, usersDto.Login),
                        new Claim(ClaimTypes.Role, usersDto.Role)
                    };

                    var identity = new ClaimsIdentity(claimList, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return new JsonResult(new
                    {
                        login = usersDto.Login,
                        firstName = usersDto.FirstName,
                        lastName = usersDto.LastName,
                        department = usersDto.Department,
                        role = usersDto.Role,
                        isLogged = true
                    });
                }
                else
                {
                    return StatusCode(401, "Aaaajjjjjj, Login or Password is incorrect");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }

        [HttpGet]
        [CookieAuth]
        public async Task<IActionResult> LoginStatusAsync()
        {
            try
            {
                var usersDto = await _usersServices.GetAsync(_httpContextAccessor.HttpContext.User.Identity.Name);
                
                return new JsonResult(new
                {
                    login = usersDto.Login,
                    firstName = usersDto.FirstName,
                    lastName = usersDto.LastName,
                    department = usersDto.Department,
                    role = usersDto.Role,
                    isLogged = true
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error123: {ex.Message}");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> LogoutAsync()
        {
            try
            {
                await HttpContext.SignOutAsync();
                //return Content("Wylogowanie użytkownika...");
                return StatusCode(200, "Wylogowano...");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }

        [HttpGet]
        [CookieAuth]
        [Authorize("Admin")]
        public async Task<IActionResult> GetAllAsync()
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

            var result = await _usersServices.GetAsync(new SqlPagedQuery<Users>
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
        
        // GET: UsersController/Details/5
        public ActionResult Details(int id)
        {
            return Content("Details....");
        }

        // GET: UsersController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsersController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UsersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsersController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UsersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [CookieAuth]
        public IActionResult Forbidden()
        {
            return Content("Eeeejjj, masz brak uprawnień...");
        }



        public class LoginData
        {
            public string Login { get; set; }

            public string Password { get; set; }
        }
    }
}
