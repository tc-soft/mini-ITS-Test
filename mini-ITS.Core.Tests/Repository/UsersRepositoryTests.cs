using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mini_ITS.Core.Repository;
using mini_ITS.Core.Services;
using mini_ITS.Core.Options;
using Moq;
using mini_ITS.Core.Database;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace mini_ITS.Core.Tests.Repository
{
    [TestFixture]
    public class UsersRepositoryTests
    {
        private IOptions<DatabaseOptions> _databaseOptions;
        private ISqlConnectionString _sqlConnectionString;
        private UsersRepository _usersRepository;
        
        [OneTimeSetUp]
        public void init()
        {
            var _path = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "mini-ITS.Web");

            var configuration = new ConfigurationBuilder()
               .SetBasePath(_path)
               .AddJsonFile("appsettings.json", false)
               .Build();

            _databaseOptions = Microsoft.Extensions.Options.Options.Create(configuration.GetSection("DatabaseOptions").Get<DatabaseOptions>());
            _sqlConnectionString = new SqlConnectionString(_databaseOptions);
            _usersRepository = new UsersRepository(_sqlConnectionString);
        }

        [Test]
        public async Task GetAsync_CheckAll()
        {
            var users = await _usersRepository.GetAsync();
            TestContext.Out.WriteLine($"Current row's : {users.Count()}");
            Assert.GreaterOrEqual(users.Count(), 50);
        }

        [TestCase("bartebri")]
        [TestCase("atkincol")]
        [TestCase("kirbyiza")]
        [TestCase("trevidor")]
        public async Task GetAsync_Login(string login)
        {
            var users = await _usersRepository.GetAsync(login);
            TestContext.Out.WriteLine($"Id         : {users.Id}");
            TestContext.Out.WriteLine($"Login      : {users.Login}");
            TestContext.Out.WriteLine($"FirstName  : {users.FirstName}");
            TestContext.Out.WriteLine($"LastName   : {users.LastName}");
            TestContext.Out.WriteLine($"Phone      : {users.Phone}");
            TestContext.Out.WriteLine($"Department : {users.Department}");

            Assert.IsTrue(users.Login == login);
        }

        [TestCase("5ee56913-7441-4305-8b31-bc86584fff47")]
        [TestCase("3131c3ea-5607-4fa0-b9d7-712ff41baa4e")]
        [TestCase("dfe4d2bf-08ea-4d86-9ccd-4e1ce3459c48")]
        [TestCase("99fcf2cf-9080-4c61-bd3d-66f78ce4e39f")]
        public async Task GetAsync_Id(Guid id)
        {
            var users = await _usersRepository.GetAsync(id);
            Assert.IsTrue(users.Id == id, "Jest konto : {id}");
        }
    }
}