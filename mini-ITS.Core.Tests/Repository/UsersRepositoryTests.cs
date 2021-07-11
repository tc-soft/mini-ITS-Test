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
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Tests.Repository
{
    [TestFixture]
    public class UsersRepositoryTests
    {
        private IOptions<DatabaseOptions> _databaseOptions;
        private ISqlConnectionString _sqlConnectionString;
        private UsersRepository _usersRepository;

        [OneTimeSetUp]
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
        }

        [Test]
        public async Task GetAsync_CheckAll()
        {
            var users = await _usersRepository.GetAsync();
            TestContext.Out.WriteLine($"Row's : {users.Count()}");
            
            Assert.That(users, Is.TypeOf<List<Models.Users>>(), "ERROR : return is not type of <List<Models.Users>>()");
            Assert.GreaterOrEqual(users.Count(), 50, "ERROR");

            foreach (var item in users)
            {
                if (item.FirstName.Length >= 3 && item.LastName.Length >=5)
                {
                    var login = $"{item.LastName.Substring(0, 5).ToLower()}{item.FirstName.Substring(0, 3).ToLower()}";
                    if (login == item.Login)
                    {
                        TestContext.Out.WriteLine($"User to test: {item.Login}, calculate to {login} - OK");
                    }
                    else if (item.Login == "admin")
                    {
                        TestContext.Out.WriteLine($"User to test: {item.Login}, calculate to ADMIN");
                    } 
                    else
                    {
                        TestContext.Out.WriteLine($"User to test: {item.Login}, calculate to {login} - ERROR");
                        Assert.Fail($"Error, user: { item.Login} not valid.");
                    }
                }
                else
                {
                    TestContext.Out.WriteLine($"User to test: {item.Login}, calculate to '{item.FirstName} {item.LastName}' - CANCEL");
                }
            }
        }

        [TestCase("bartlbri")]
        [TestCase("atkincol")]
        [TestCase("kirbyisa")]
        [TestCase("trevidor")]
        public async Task GetAsync_CheckLogin(string login)
        {
            var users = await _usersRepository.GetAsync(login);
            TestContext.Out.WriteLine($"Id         : {users.Id}");
            TestContext.Out.WriteLine($"Login      : {users.Login}");
            TestContext.Out.WriteLine($"FirstName  : {users.FirstName}");
            TestContext.Out.WriteLine($"LastName   : {users.LastName}");
            TestContext.Out.WriteLine($"Department : {users.Department}");
            TestContext.Out.WriteLine($"Phone      : {users.Email}");
            TestContext.Out.WriteLine($"Phone      : {users.Phone}");
            TestContext.Out.WriteLine($"Phone      : {users.Role}");
            TestContext.Out.WriteLine($"Phone      : {users.PasswordHash}");

            Assert.IsTrue(users.Login == login);
        }

        [TestCase("5ee56913-7441-4305-8b31-bc86584fff47")]
        [TestCase("3131c3ea-5607-4fa0-b9d7-712ff41baa4e")]
        [TestCase("dfe4d2bf-08ea-4d86-9ccd-4e1ce3459c48")]
        [TestCase("99fcf2cf-9080-4c61-bd3d-66f78ce4e39f")]
        public async Task GetAsync_CheckId(Guid id)
        {
            var users = await _usersRepository.GetAsync(id);
            Assert.IsTrue(users.Id == id, "Jest konto : {id}");
        }

        [Test, Combinatorial]
        public async Task GetAsync_CheckRoleDepartment(
            [Values(null, "User", "Manager")] string role,
            [Values(null, "Sales", "Research")] string department)
        {
            var users = await _usersRepository.GetAsync(role, department);
            TestContext.Out.WriteLine($"Row's : {users.Count()}");

            foreach (var item in users)
            {
                TestContext.Out.WriteLine($"User: {item.Login}, Role={item.Role} Department={item.Department}");
                if (role is not null) Assert.AreEqual(role, item.Role);
                if (department is not null) Assert.AreEqual(department, item.Department);
            }
        }

        [Test, Combinatorial]
        public async Task GetAsync_CheckSqlQueryCondition(
            [Values(null, "User", "Manager")] string role,
            [Values(null, "Sales", "Research")] string department)
        {
            var sqlQueryConditionList = new List<SqlQueryCondition>()
            {
                new SqlQueryCondition
                {
                    Name = "Role",
                    Operator = SqlQueryOperator.Equal,
                    Value = role
                },
                new SqlQueryCondition
                {
                    Name = "Department",
                    Operator = SqlQueryOperator.Equal,
                    Value = department
                }
            };

            var users = await _usersRepository.GetAsync(sqlQueryConditionList);
            TestContext.Out.WriteLine($"Row's : {users.Count()}");

            Assert.That(users.Count() > 0, "ERROR - users is empty");

            Assert.That(users, Is.TypeOf<List<Users>>(), "ERROR return type. Must be : List<Users>");
            Assert.That(users, Is.All.InstanceOf<Users>(), "ERROR - return Instance must be <Users>");
            Assert.That(users, Is.Unique);

            foreach (var item in users)
            {
                TestContext.Out.WriteLine($"User: {item.Login}, Role={item.Role} Department={item.Department}");
                if (role is not null) Assert.That(item.Role, Is.EqualTo(role), "ERROR - Role is not equal");
                if (department is not null) Assert.That(item.Department, Is.EqualTo(department), "ERROR - Department is not equal");

                Assert.IsNotNull(item.Id, $"ERROR - {nameof(item.Id)} is null");
                Assert.IsNotNull(item.Login, $"ERROR - {nameof(item.Login)} is null");
                Assert.IsNotNull(item.FirstName, $"ERROR - {nameof(item.FirstName)} is null");
                Assert.IsNotNull(item.LastName, $"ERROR - {nameof(item.LastName)} is null");
                Assert.IsNotNull(item.Department, $"ERROR - {nameof(item.Department)} is null");
                Assert.IsNotNull(item.Email, $"ERROR - {nameof(item.Email)} is null");
                Assert.IsNotNull(item.Role, $"ERROR - {nameof(item.Role)} is null");
                Assert.IsNotNull(item.PasswordHash, $"ERROR - {nameof(item.PasswordHash)} is null");
            }
        }

        [TestCase("Sales", null, 5, "Login", "DESC")]
        [TestCase("Development", "User",2, "FirstName", "ASC")]
        [TestCase(null, null, 10, "Email", "ASC")]
        public async Task GetAsync_SqlPagedQuery(
            string department,
            string role,
            int resultsPerPage,
            string sortColumnName,
            string sortDirection)
        {
            var sqlQueryConditionList = new List<SqlQueryCondition>()
            {
                new SqlQueryCondition
                {
                    Name = "Department",
                    Operator = SqlQueryOperator.Equal,
                    Value = department
                },
                new SqlQueryCondition
                {
                    Name = "Role",
                    Operator = SqlQueryOperator.Equal,
                    Value = role
                }
            };

            var sqlPagedQuery = new SqlPagedQuery<Users>
            {
                Filter = sqlQueryConditionList,
                SortColumnName = sortColumnName,
                SortDirection = sortDirection,
                Page = 1,
                ResultsPerPage = resultsPerPage
            };

            var usersList = await _usersRepository.GetAsync(sqlPagedQuery);
            
            for (int i = 1; i <= usersList.TotalPages; i++)
            {
                sqlPagedQuery.Page = i;
                var users = await _usersRepository.GetAsync(sqlPagedQuery);
                TestContext.Out.WriteLine($"\nPage {users.CurrentPage}/{usersList.TotalPages} - ResultsPerPage={users.ResultsPerPage}, TotalResults={users.TotalResults} (Login, FirstName, Department, Email, Role)");

                Assert.That(users.Results.Count() > 0, "ERROR - users is empty");
                Assert.That(users, Is.TypeOf<SqlPagedResult<Users>>(), "ERROR - return type");
                Assert.That(users.Results, Is.All.InstanceOf<Users>(), "ERROR - All.InstanceOf<Users>()");
                Assert.That(users.Results, Is.Unique);

                switch (sqlPagedQuery.SortDirection)
                {
                    case "ASC":
                        Assert.That(users.Results, Is.Ordered.Ascending.By(sqlPagedQuery.SortColumnName), "ERROR - sort");
                        break;
                    case "DESC":
                        Assert.That(users.Results, Is.Ordered.Descending.By(sqlPagedQuery.SortColumnName), "ERROR - sort");
                        break;
                    default:
                        Assert.Fail("ERROR - SortDirection is not T-SQL");
                        break;
                };

                foreach (var item in users.Results)
                {
                    TestContext.Out.WriteLine($"{item.Login}\t{item.FirstName}\t{(department is null ? "*" : department)}\t{item.Email}\t{(role is null ? "*" : role)}");

                    if (role is not null) Assert.That(item.Role, Is.EqualTo(role), "ERROR - Role is not equal");
                    if (department is not null) Assert.That(item.Department, Is.EqualTo(department), "ERROR - Department is not equal");

                    Assert.IsNotNull(item.Id, $"ERROR - {nameof(item.Id)} is null");
                    Assert.IsNotNull(item.Login, $"ERROR - {nameof(item.Login)} is null");
                    Assert.IsNotNull(item.FirstName, $"ERROR - {nameof(item.FirstName)} is null");
                    Assert.IsNotNull(item.LastName, $"ERROR - {nameof(item.LastName)} is null");
                    Assert.IsNotNull(item.Department, $"ERROR - {nameof(item.Department)} is null");
                    Assert.IsNotNull(item.Email, $"ERROR - {nameof(item.Email)} is null");
                    Assert.IsNotNull(item.Role, $"ERROR - {nameof(item.Role)} is null");
                    Assert.IsNotNull(item.PasswordHash, $"ERROR - {nameof(item.PasswordHash)} is null");
                }
            }
        }
    }
}