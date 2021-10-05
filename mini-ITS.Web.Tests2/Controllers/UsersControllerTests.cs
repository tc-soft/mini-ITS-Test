using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using mini_ITS.Core.Database;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Models;
using mini_ITS.Core.Options;
using mini_ITS.Core.Repository;
using mini_ITS.Core.Services;
using mini_ITS.Web.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static mini_ITS.Web.Controllers.UsersController;

namespace mini_ITS.Web.Tests2.Controllers
{
    public class UsersControllerTests
    {
        private IOptions<DatabaseOptions> _databaseOptions;
        private ISqlConnectionString _sqlConnectionString;

        private IMapper _mapper;
        private IPasswordHasher<Users> _hasher;
        private IHttpContextAccessor _httpContextAccessor;
        private IUsersRepository _usersRepository;
        private IUsersService _usersService;
        private UsersController _usersController;

        public UsersControllerTests()
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
            _usersService = new UsersService(_usersRepository, _mapper, _hasher);

            //_usersController = new UsersController(_usersService, _mapper, _httpContextAccessor);
        }


        [Fact]
        public async Task Test1()
        {
            var result = await _usersController.LoginAsync(new LoginData { 
                Login = "admin",
                Password = "admin"
            });

            //TestContext.Out.WriteLine($"Result: {result.Result}");

            Assert.IsType<OkObjectResult>(result);

        }
    }
}
