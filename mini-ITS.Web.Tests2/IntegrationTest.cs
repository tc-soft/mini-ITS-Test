using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using mini_ITS.Core.Database;
using mini_ITS.Core.Models;
using mini_ITS.Core.Options;
using mini_ITS.Core.Repository;
using mini_ITS.Core.Services;
using mini_ITS.Web.Mapper;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace mini_ITS.Web.Tests2
{
    public class IntegrationTest
    {
        protected readonly HttpClient TestClient;
        //private IConfiguration Configuration { get; }

        protected IntegrationTest()
        {
            //using Microsoft.AspNetCore.Mvc.Testing;
            var appFactory = new WebApplicationFactory<Startup>();
                //.WithWebHostBuilder(builder =>
                //{
                //    builder.ConfigureServices(services =>
                //    {
                //        //IConfiguration Configuration;
                        
                //        //services.Configure<DatabaseOptions>(Configuration.GetSection("DatabaseOptions"));

                //        services.AddSingleton<ISqlConnectionString, SqlConnectionString>();
                //        services.AddScoped<IUsersRepository, UsersRepository>();
                //        services.AddScoped<IUsersService, UsersService>();

                //        //potrzebne do Service
                //        services.AddSingleton(AutoMapperConfig.GetMapper());
                //        //potrzebne do Service
                //        services.AddSingleton<IPasswordHasher<Users>, PasswordHasher<Users>>();

                //        //============= Do autoryzacji
                //        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                //            .AddCookie(options =>
                //            {
                //                options.Cookie.HttpOnly = true;
                //                options.Cookie.Name = "mini-ITS.SessionCookie";
                //                options.LoginPath = new PathString("/Users/Login1");
                //                options.AccessDeniedPath = new PathString("/Users/Forbidden");
                //                options.ExpireTimeSpan = TimeSpan.FromDays(2);
                //            });

                //        services.AddAuthorization(options =>
                //        {
                //            options.AddPolicy("Admin", policy =>
                //            {
                //                policy.RequireAuthenticatedUser()
                //                      .RequireRole("Administrator");
                //            });

                //            options.AddPolicy("Manager", policy =>
                //            {
                //                policy.RequireAuthenticatedUser()
                //                      .RequireRole("Manager");
                //            });

                //            options.AddPolicy("User", policy =>
                //            {
                //                policy.RequireAuthenticatedUser()
                //                      .RequireRole("User");
                //            });
                //        });

                //        //================= do kontrolera UsersController
                //        services.AddHttpContextAccessor();

                //        services.AddControllersWithViews();

                //        //ValidateAntiForgeryToken
                //        services.AddAntiforgery(o => {
                //            o.Cookie.Name = "X-CSRF-TOKEN";
                //        });

                //        // In production, the React files will be served from this directory
                //        services.AddSpaStaticFiles(configuration =>
                //        {
                //            configuration.RootPath = "ClientApp/build";
                //        });

                //    });
                //});

            TestClient = appFactory.CreateClient();
        }

        protected async Task LoginAsync()
        {
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Users.GetAll, new LoginData
            {
                Login = "admin",
                Password = "admin"
            });

            var test = response;
        }

        protected async Task<PostResponse> CreatePostAsync()
        {
            var person = new LoginData {
                Login = "admin",
                Password = "admin"
            };

            var json = JsonConvert.SerializeObject(person);
            var request = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Users.GetAll, request);

            return JsonConvert.DeserializeObject<PostResponse>(
                await response.Content.ReadAsStringAsync()
                );

        }
    }
}

public class LoginData
{
    public string Login { get; set; }
    public string Password { get; set; }
}

public class PostResponse
{
    public string Login;
    public string firstName;
    public string lastName;
    public string department;
    public string role;
    public bool isLogged;
}