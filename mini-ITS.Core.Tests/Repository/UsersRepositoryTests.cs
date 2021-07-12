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
                TestContext.Out.WriteLine($"\nPage {users.CurrentPage}/{usersList.TotalPages} - ResultsPerPage={users.ResultsPerPage}, TotalResults={users.TotalResults}");
                TestContext.Out.WriteLine($"{("Login").PadRight(10)}{("FirstName").PadRight(20)}{("Department").PadRight(20)}{("Email").PadRight(40)}{("Role").PadRight(20)}");

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
                    TestContext.Out.WriteLine($"{item.Login.PadRight(10)}{item.FirstName.PadRight(20)}{(department is null ? "*".PadRight(20) : department.PadRight(20))}{item.Email.PadRight(40)}{(role is null ? "*".PadRight(20) : role.PadRight(20))}");

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

        [TestCaseSource(typeof(UsersRepositoryTestsData), nameof(UsersRepositoryTestsData.UsersCases))]
        public async Task GetAsync_CheckId(Users users)
        {
            var user = await _usersRepository.GetAsync(users.Id);

            TestContext.Out.WriteLine($"Id         : {user.Id}");
            TestContext.Out.WriteLine($"Login      : {user.Login}");
            TestContext.Out.WriteLine($"FirstName  : {user.FirstName}");
            TestContext.Out.WriteLine($"LastName   : {user.LastName}");
            TestContext.Out.WriteLine($"Department : {user.Department}");
            TestContext.Out.WriteLine($"Phone      : {user.Email}");
            TestContext.Out.WriteLine($"Phone      : {user.Phone}");
            TestContext.Out.WriteLine($"Phone      : {user.Role}");
            TestContext.Out.WriteLine($"Phone      : {user.PasswordHash}");

            Assert.That(user, Is.TypeOf<Users>(), "ERROR - return type");

            Assert.That(user.Id, Is.EqualTo(users.Id), "ERROR - Id is not equal");
            Assert.That(user.Login, Is.EqualTo(users.Login), "ERROR - Login is not equal");
            Assert.That(user.FirstName, Is.EqualTo(users.FirstName), "ERROR - FirstName is not equal");
            Assert.That(user.LastName, Is.EqualTo(users.LastName), "ERROR - LastName is not equal");
            Assert.That(user.Department, Is.EqualTo(users.Department), "ERROR - Department is not equal");
            Assert.That(user.Email, Is.EqualTo(users.Email), "ERROR - Email is not equal");
            Assert.That(user.Phone, Is.EqualTo(users.Phone), "ERROR - Phone is not equal");
            Assert.That(user.Role, Is.EqualTo(users.Role), "ERROR - Role is not equal");
            Assert.That(user.PasswordHash, Is.EqualTo(users.PasswordHash), "ERROR - PasswordHash is not equal");
        }

        [TestCaseSource(typeof(UsersRepositoryTestsData), nameof(UsersRepositoryTestsData.UsersCases))]
        public async Task GetAsync_CheckLogin(Users users)
        {
            var user = await _usersRepository.GetAsync(users.Login);

            TestContext.Out.WriteLine($"Id         : {user.Id}");
            TestContext.Out.WriteLine($"Login      : {user.Login}");
            TestContext.Out.WriteLine($"FirstName  : {user.FirstName}");
            TestContext.Out.WriteLine($"LastName   : {user.LastName}");
            TestContext.Out.WriteLine($"Department : {user.Department}");
            TestContext.Out.WriteLine($"Phone      : {user.Email}");
            TestContext.Out.WriteLine($"Phone      : {user.Phone}");
            TestContext.Out.WriteLine($"Phone      : {user.Role}");
            TestContext.Out.WriteLine($"Phone      : {user.PasswordHash}");

            Assert.That(user, Is.TypeOf<Users>(), "ERROR - return type");

            Assert.That(user.Id, Is.EqualTo(users.Id), "ERROR - Id is not equal");
            Assert.That(user.Login, Is.EqualTo(users.Login), "ERROR - Login is not equal");
            Assert.That(user.FirstName, Is.EqualTo(users.FirstName), "ERROR - FirstName is not equal");
            Assert.That(user.LastName, Is.EqualTo(users.LastName), "ERROR - LastName is not equal");
            Assert.That(user.Department, Is.EqualTo(users.Department), "ERROR - Department is not equal");
            Assert.That(user.Email, Is.EqualTo(users.Email), "ERROR - Email is not equal");
            Assert.That(user.Phone, Is.EqualTo(users.Phone), "ERROR - Phone is not equal");
            Assert.That(user.Role, Is.EqualTo(users.Role), "ERROR - Role is not equal");
            Assert.That(user.PasswordHash, Is.EqualTo(users.PasswordHash), "ERROR - PasswordHash is not equal");
        }
    }
}