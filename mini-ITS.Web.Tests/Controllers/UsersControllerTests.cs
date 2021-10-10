using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using mini_ITS.Web.Controllers;
using Moq;
using mini_ITS.Core.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using static mini_ITS.Web.Controllers.UsersController;
using Microsoft.AspNetCore.Mvc;
using mini_ITS.Core.Repository;
using Microsoft.AspNetCore.Identity;
using mini_ITS.Core.Models;
using mini_ITS.Core.Database;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using mini_ITS.Core.Options;
using mini_ITS.Core.Dto;
using RestSharp;
using System.Net;
using RestSharp.Serialization.Json;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;

namespace mini_ITS.Web.Tests.Controllers
{
    public class UsersControllerTests
    {
        private IOptions<DatabaseOptions> _databaseOptions;
        private ISqlConnectionString _sqlConnectionString;
        private IUsersRepository _usersRepository;
        private IMapper _mapper;
        private IPasswordHasher<Users> _hasher;
        private IUsersService _usersServices;

        private Mock<IHttpContextAccessor> _httpContextAccessor;
        //private IHttpContextAccessor _httpContextAccessor;
        private UsersController _usersController;

        private TestServer _server;
        private HttpClient _client;
        private IHostBuilder _hostBuilder;

        [SetUp]
        public void Init()
        {
            var _path = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "mini-ITS.Web");

            var configuration = new ConfigurationBuilder()
               .SetBasePath(_path)
               .AddJsonFile("appsettings.json", false)
               .Build();

            _databaseOptions = Microsoft.Extensions.Options.Options.Create(configuration.GetSection("DatabaseOptions").Get<DatabaseOptions>());
            _sqlConnectionString = new SqlConnectionString(_databaseOptions);
            _usersRepository = new UsersRepository(_sqlConnectionString);
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UsersDto, Users>();
                cfg.CreateMap<Users, UsersDto>();
            }).CreateMapper();
            _hasher = new PasswordHasher<Users>();

            _usersServices = new UsersService(_usersRepository, _mapper, _hasher);

            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            //_httpContextAccessor = new HttpContextAccessor();

            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object)null));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);

            _usersController = new UsersController(_usersServices, _mapper, _httpContextAccessor.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        // How mock RequestServices?
                        RequestServices = serviceProviderMock.Object
                    }
                }

            };


            //usersController = new Mock<UsersController>();
            //_usersController = new UsersController(_usersService, _mapper, _httpContextAccessor);

            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            //_server = new TestServer(new WebHostBuilder().UseEnvironment("Development").UseStartup<Startup>());

            _client = _server.CreateClient();


            _hostBuilder = new HostBuilder().ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.Configure(app => app.Run(async ctx =>
                await ctx.Response.WriteAsync("Hello World!")));
            });


        }

        //[Test]
        public void LoginWithRestSharp()
        {
            //RestClient client = new RestClient("https://localhost:44375");
            //RestRequest request = new RestRequest("Users/Login", Method.POST);

            var client = new RestClient("https://localhost:44375/Users/Login");
            client.Timeout = 3000;

            var request = new RestRequest(Method.POST);
            request.AddJsonBody(new LoginData
            {
                Login = "admin",
                Password = "admin"
            });

            var response = client.Execute(request);
            TestContext.Out.WriteLine($"Result: {response.Content}");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(response.ContentType.StartsWith("application/json"));

            //var issues = new JsonDeserializer().Deserialize<List<IssueResponse>>(response);

            //TestContext.Out.WriteLine($"Result: {issues}");

            Assert.Pass();
        }

        //[Test]
        public async Task LoginWithMock()
        {
            var result = await _usersController.LoginAsync(new LoginData
            {
                Login = "admin",
                Password = "admin"
            });

            JsonResult jsonResult = result as JsonResult;
            var statusCodeResult = (IStatusCodeActionResult)result;


            TestContext.Out.WriteLine($"result: {statusCodeResult.StatusCode}");
            TestContext.Out.WriteLine($"result: {(result as OkObjectResult).StatusCode}");

            //foreach (var item in jsonResult.Data as dynamic)
            //{
            //    ((int)item.Id).ShouldBe(expected Id value);
            //    ((string)item.name).ShouldBe("expected name value");
            //}

            //Assert.That(
            //    result.Data as List<IProduct>,
            //    Is.EqualTo(mockProductsData)
            //    );
            //Assert.NotNull(result);
            //Assert.True(result is OkObjectResult);
            //Assert.IsType<JsonResult>(result.Value);
            //Assert.Equal(StatusCodes.Status200OK, result.StatusCode);

            Assert.AreEqual(StatusCodes.Status200OK, statusCodeResult.StatusCode);

            TestContext.Out.WriteLine($"Tu OK");
            var result2 = await _usersController.LoginStatusAsync();

        }

        //[Test]
        public async Task GetUsers()
        {
            var response = await _client.GetAsync("Users/Index");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        //[Test]
        public async Task LoginWithTestHost()
        {
            var formData = new Dictionary<string, string>
            {
                {"Login", "admin"},
                {"Password", "admin"}
            };

            var loginData = new LoginData
            {
                Login = "admin",
                Password = "admin"
            };

            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, "Users/Login")
            {
                Content = new FormUrlEncodedContent(formData)
            };

            //var response = await _client.SendAsync(postRequest);

            var content = JsonConvert.SerializeObject(loginData);
            var stringContent = new StringContent(content, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("https://localhost:44375/Users/Login", stringContent);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        //[Test]
        public async Task Test2()
        {
            // Build and start the IHost
            var host = await _hostBuilder.StartAsync();

            // Create an HttpClient to send requests to the TestServer
            var client = host.GetTestClient();

            var response = await client.GetAsync("/");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("Hello World!", responseString);

        }
    }
}
