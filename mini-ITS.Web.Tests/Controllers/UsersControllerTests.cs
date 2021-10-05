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

namespace mini_ITS.Web.Tests.Controllers
{
    public class UsersControllerTests
    {
        private IUsersService usersService;
        private UsersController usersController;
        private IUsersRepository _usersRepository;
        //private Mock<IOptions<DatabaseOptions>> _databaseOptions;
        private Mock<IPasswordHasher<Users>> hasher;
        private Mock<IMapper> mapper;
        private Mock<IHttpContextAccessor> httpContextAccessor;

        private IOptions<DatabaseOptions> _databaseOptions;
        private ISqlConnectionString _sqlConnectionString;

        private IMapper _mapper;
        private IPasswordHasher<Users> _hasher;
        //private IUsersRepository _usersRepository;
        //private IUsersService _usersService;
        //private UsersController _usersController;
        //private IHttpContextAccessor _httpContextAccessor;

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
            usersService = new UsersService(_usersRepository, _mapper, _hasher);



            //mapper = new Mock<IMapper>();

            httpContextAccessor = new Mock<IHttpContextAccessor>();
            //hasher = new Mock<IPasswordHasher<Users>>();

            //usersService = new Mock<IUsersService>();
            //usersController = new UsersController(usersService.Object, mapper.Object, httpContextAccessor.Object);

            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object)null));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);

            usersController = new UsersController(usersService, _mapper, httpContextAccessor.Object)
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

        }

        [Test]
        public void LoginWithRestSharp()
        {
            RestClient client = new RestClient("https://localhost:44375");
            RestRequest request = new RestRequest("Users/Login", Method.POST);

            //var client = new RestClient("https://localhost:44375/Users/Login");
            client.Timeout = 3000;
            
            //var request = new RestRequest(Method.POST);
            request.AddJsonBody(new LoginData
            {
                Login = "admin",
                Password = "admin"
            });

            var response = client.Execute(request);
            TestContext.Out.WriteLine($"Result: {response.Content}");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(response.ContentType.StartsWith("application/json"));

            var issues = new JsonDeserializer().Deserialize<List<IssueResponse>>(response);

            TestContext.Out.WriteLine($"Result: {issues}");

            Assert.Pass();
        }

        [Test]
        public async Task LoginWithMock()
        {
            var result = await usersController.LoginAsync(new LoginData
            {
                Login = "admin",
                Password = "admini"
            });

            var resultStatusCode = ((JsonResult)result).Value;
            //var resultValue = ((ObjectResult)result).Value;

            TestContext.Out.WriteLine($"resultStatusCode: {resultStatusCode}");
            //TestContext.Out.WriteLine($"resultValue: {resultValue}");

            Assert.AreEqual(HttpStatusCode.OK, result);
        }
    }
}
public class IssueResponse
{
    public string Login;
    public string firstName;
    public string lastName;
    public string department;
    public string role;
    public bool isLogged;
}