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

namespace mini_ITS.Web.Tests.Controllers
{
    public class UsersControllerTests
    {
        private UsersController usersController;
        private Mock<IUsersRepository> usersRepository;
        private Mock<IUsersService> usersService;
        private Mock<IOptions<DatabaseOptions>> _databaseOptions;
        private Mock<IPasswordHasher<Users>> hasher;
        private Mock<IMapper> mapper;
        private Mock<IHttpContextAccessor> httpContextAccessor;

        //private IOptions<DatabaseOptions> _databaseOptions;
        //private ISqlConnectionString _sqlConnectionString;

        //private IMapper _mapper;
        //private IPasswordHasher<Users> _hasher;
        //private IUsersRepository _usersRepository;
        //private IUsersService _usersService;
        //private UsersController _usersController;
        //private IHttpContextAccessor _httpContextAccessor;

        [SetUp]
        public void Init()
        {
            mapper = new Mock<IMapper>();
            httpContextAccessor = new Mock<IHttpContextAccessor>();
            hasher = new Mock<IPasswordHasher<Users>>();

            usersService = new Mock<IUsersService>();
            usersController = new UsersController(usersService.Object, mapper.Object, httpContextAccessor.Object);

            //usersController = new Mock<UsersController>();

            //var _path = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "mini-ITS.Web");

            //var configuration = new ConfigurationBuilder()
            //   .SetBasePath(_path)
            //   .AddJsonFile("appsettings.json", false)
            //   .Build();

            //_databaseOptions = Microsoft.Extensions.Options.Options.Create(configuration.GetSection("DatabaseOptions").Get<DatabaseOptions>());
            //_sqlConnectionString = new SqlConnectionString(_databaseOptions);
            //_usersRepository = new UsersRepository(_sqlConnectionString);
            //_mapper = new MapperConfiguration(cfg =>
            //{
            //    cfg.CreateMap<UsersDto, Users>();
            //    cfg.CreateMap<Users, UsersDto>();
            //}).CreateMapper();
            //_hasher = new PasswordHasher<Users>();
            //_usersService = new UsersService(_usersRepository, _mapper, _hasher);
            //_usersController = new UsersController(_usersService, _mapper, _httpContextAccessor);
        }

        [Test]
        public async Task LoginAsync()
        {
            //var result = usersController.Setup(p => p.LoginAsync(new LoginData
            //{
            //    Login = "admin",
            //    Password = "admin"
            //}));

            var result = await usersController.LoginAsync(new LoginData
            {
                Login = "admin",
                Password = "admin"
            });

            //var actualResult = (await usersController.LoginAsync(It.IsAny<Guid>()) as
            //OkNegotiatedContentResult<IEnumerable<MyAccount>>).Content;

            //var result = usersController.LoginAsync(loginData).Result;
            var resultStatusCode = ((ObjectResult)result).StatusCode;

            TestContext.Out.WriteLine($"Result: {resultStatusCode}");

            Assert.Pass();
        }

        [Test]
        public void Login()
        {
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

            var issues = new JsonDeserializer().Deserialize<List<IssueResponse>>(response);

            TestContext.Out.WriteLine($"Result: {issues}");

            Assert.Pass();
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