using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mini_ITS.Core.Database;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Models;
using mini_ITS.Core.Services;
using mini_ITS.Web.Framework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace mini_ITS.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersServices;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsersController(IUsersService usersServices, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _usersServices = usersServices;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody] LoginData loginData )
        {
            try
            {
                //_usersServices.SetPasswordAsync("admin", "admin");
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
                    return StatusCode(401, "Login or password is incorrect");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }

        [HttpGet]
        [CookieAuth]
        [ValidateAntiForgeryToken]
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
                throw new Exception($"Error: {ex.Message}");
            }
        }

        [HttpDelete]
        [CookieAuth]
        public async Task<IActionResult> LogoutAsync()
        {
            try
            {
                await HttpContext.SignOutAsync();
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
        public async Task<IActionResult> IndexAsync([FromQuery] SqlPagedQuery<Users> sqlPagedQuery)
        {
            try
            {
                var result = await _usersServices.GetAsync(sqlPagedQuery);
                var users = _mapper.Map<IEnumerable<Users>>(result.Results);
                var sqlPagedResult = SqlPagedResult<Users>.From(result, users);

                return Ok(sqlPagedResult);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] UsersDto usersDto)
        {
            try
            {
                await _usersServices.CreateAsync(usersDto);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("Users/Edit/{id:guid}")]
        [CookieAuth]
        [Authorize("Admin")]
        public async Task<IActionResult> EditAsync(Guid? id)
        {
            try
            {
                if (id.HasValue)
                {
                    var user = await _usersServices.GetAsync((Guid)id);
                    user.PasswordHash = "";
                    return Ok(user);
                }
                else
                {
                    return StatusCode(500, $"Error: id is null");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }

        [HttpPut("Users/Edit/{id:guid}")]
        [CookieAuth]
        [Authorize("Admin")]
        public async Task<IActionResult> EditAsync([FromBody] UsersDto usersDto)
        {
            try
            {
                if (usersDto is not null)
                {
                    await _usersServices.UpdateAsync(usersDto);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPatch("Users/ChangePassword/{id:guid}")]
        [CookieAuth]
        [Authorize("Admin")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePassword changePassword)
        {
            try
            {
                if (changePassword.OldPassword is not null &&
                    changePassword.NewPassword is not null &&
                    await _usersServices.LoginAsync(changePassword.Login, changePassword.OldPassword))  
                {
                    await _usersServices.ChangePasswordAsync(changePassword.Login, changePassword.OldPassword, changePassword.NewPassword);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpDelete("Users/Delete/{id:guid}")]
        [CookieAuth]
        [Authorize("Admin")]
        public async Task<IActionResult> DeleteAsync(Guid? id)
        {
            try
            {
                if (id.HasValue)
                {
                    await _usersServices.DeleteAsync((Guid)id);
                    return Ok();
                }
                else
                {
                    return StatusCode(500, $"Error: id is null");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
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

        public class ChangePassword
        {
            public string Login { get; set; }
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }

    }
}
