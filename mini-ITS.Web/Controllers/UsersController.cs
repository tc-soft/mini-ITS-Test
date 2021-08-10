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
        // GET: UsersController
        public async Task<IActionResult> LoginAsync()
        {
            //var user = await _usersServices.GetAsync("ciszetad");

            var claimList = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "ciszetad"),
                new Claim(ClaimTypes.Role, "Administrator")
            };

            //var claimList = new List<Claim>();
            //claimList.Add(new Claim(ClaimTypes.Name, "ciszetad"));
            //claimList.Add(new Claim(ClaimTypes.Role, "Administrator"));

            var claimsIdentity = new ClaimsIdentity(claimList, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            //sprawdzić co się stanie po 10 minutach
            //var authProperties = new AuthenticationProperties
            //{
            //    ExpiresUtc = DateTime.Now.AddMinutes(10),
            //};

            //await HttpContext.SignInAsync(
            //    CookieAuthenticationDefaults.AuthenticationScheme,
            //    new ClaimsPrincipal(claimsIdentity),
            //    authProperties);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            return Content("Użytkownik zalogowany...");
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
    }
}
