using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public UsersController(IUsersService usersServices)
        {
            _usersServices = usersServices;
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

        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync();
            return Content("Wylogowanie użytkownika...");
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
