using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mini_ITS.Core.Repository;
using mini_ITS.Core.Services;
using Moq;
using mini_ITS.Core.Database;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using mini_ITS.Core.Options;

namespace mini_ITS.Core.Tests.Repository
{
    //[TestFixture]
    //class UsersRepositoryTests
    //{
    //    public abstract IUsersRepository UsersRepository;

    //    public abstract IStringSearcher GetStringSearcherInstance();

    //    //public UsersRepositoryTests(IUsersRepository usersRepository)
    //    //{
    //    //    _usersRepository = usersRepository;

    //    //}

    //    [SetUp]
    //    public void SetUp()
    //    {
    //        // initialize here
    //            _usersRepository = new UsersRepository;
    //    }

    //    //[SetUp]
    //    //public async Task SetupAsync()
    //    //{
    //    //    var result = await _usersRepository.GetAsync();
    //    //    Assert.Pass();
    //    //}

    //    [Test]
    //    public void Test1()
    //    {
    //        Assert.Pass();
    //    }
    //}

    [TestFixture]
    public class UsersRepositoryTests
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void GetAsync()
        {
            //var services = new ServiceCollection();
            //services.AddSingleton<ISqlConnectionString, SqlConnectionString>();

            var sqlConnectionString = Mock.Of<ISqlConnectionString>();
            var _usersRepository = new UsersRepository(sqlConnectionString);

            var users = _usersRepository.GetAsync();
            //var count = users.Result.Count();
            //Assert.GreaterOrEqual(50, count);
            Assert.IsNotNull(users);
        }
        private async Task MyTestMethod()
        {
            await Task.Run(() => throw new Exception());
        }

    }
}
