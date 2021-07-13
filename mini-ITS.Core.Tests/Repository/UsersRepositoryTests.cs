using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using mini_ITS.Core.Database;
using mini_ITS.Core.Models;
using mini_ITS.Core.Options;
using mini_ITS.Core.Repository;
using System;

namespace mini_ITS.Core.Tests.Repository
{
    [TestFixture]
    public class UsersRepositoryTests
    {
        private IOptions<DatabaseOptions> _databaseOptions;
        private ISqlConnectionString _sqlConnectionString;
        private UsersRepository _usersRepository;

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
        }

        [Test]
        public async Task GetAsync_CheckAll()
        {
            var users = await _usersRepository.GetAsync();
            TestContext.Out.WriteLine($"Row's : {users.Count()}");

            Assert.That(users.Count() > 0, "ERROR - users is empty");
            Assert.That(users, Is.TypeOf<List<Users>>(), "ERROR - return type");
            Assert.That(users, Is.All.InstanceOf<Users>(), "ERROR - each instance of <Users>()");
            Assert.That(users, Is.Ordered.Ascending.By("Login"), "ERROR - sort");
            Assert.That(users, Is.Unique);

            foreach (var item in users)
            {
                if (item.FirstName.Length >= 3 && item.LastName.Length >=5)
                {
                    var login = $"{item.LastName.Substring(0, 5).ToLower()}{item.FirstName.Substring(0, 3).ToLower()}";
                    if (login == item.Login)
                    {
                        TestContext.Out.WriteLine($"User: {item.Login}, calculate to {login} - OK");
                    }
                    else if (item.Login == "admin")
                    {
                        TestContext.Out.WriteLine($"User: {item.Login}, calculate to ADMIN");
                    } 
                    else
                    {
                        TestContext.Out.WriteLine($"User: {item.Login}, calculate to {login} - ERROR");
                        Assert.Fail($"Error, user: { item.Login} not valid.");
                    }
                }
                else
                {
                    TestContext.Out.WriteLine($"User: {item.Login}, calculate to '{item.FirstName} {item.LastName}' - CANCEL");
                }
            }
        }

        [Test, Combinatorial]
        public async Task GetAsync_CheckDepartmentRole(
            [Values(null, "Sales", "Research")] string department,
            [Values(null, "User", "Manager")] string role)
        {
            var users = await _usersRepository.GetAsync(department, role);
            TestContext.Out.WriteLine($"Row's : {users.Count()}");
            TestContext.Out.WriteLine($"{("Login").PadRight(10)}{("FirstName").PadRight(20)}{("Department").PadRight(20)}{("Email").PadRight(40)}{("Role").PadRight(20)}");

            Assert.That(users.Count() > 0, "ERROR - users is empty");
            Assert.That(users, Is.TypeOf<List<Users>>(), "ERROR - return type");
            Assert.That(users, Is.All.InstanceOf<Users>(), "ERROR - each instance of <Users>()");
            Assert.That(users, Is.Ordered.Ascending.By("Login"), "ERROR - sort");
            Assert.That(users, Is.Unique);

            foreach (var item in users)
            {
                TestContext.Out.WriteLine($"{item.Login.PadRight(10)}{item.FirstName.PadRight(20)}{(department is null ? "*".PadRight(20) : department.PadRight(20))}{item.Email.PadRight(40)}{(role is null ? "*".PadRight(20) : role.PadRight(20))}");
                if (department is not null) Assert.That(item.Department, Is.EqualTo(department), "ERROR - Department is not equal");
                if (role is not null) Assert.That(item.Role, Is.EqualTo(role), "ERROR - Role is not equal");
            }
        }

        [Test, Combinatorial]
        public async Task GetAsync_CheckSqlQueryCondition(
            [Values(null, "Sales", "Research")] string department,
            [Values(null, "User", "Manager")] string role)
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

        [TestCaseSource(typeof(UsersRepositoryTestsData), nameof(UsersRepositoryTestsData.SqlPagedQueryCases))]
        public async Task GetAsync_SqlPagedQuery(SqlPagedQuery<Users> sqlPagedQuery)
        {
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
                    foreach (var filter in sqlPagedQuery.Filter)
                    {
                        if (filter.Value is not null)
                        {
                            Assert.That(
                                item.GetType().GetProperty(filter.Name).GetValue(item, null),
                                Is.EqualTo(filter.Value),
                                $"ERROR - Filter {filter.Name} is not equal");
                        }
                    }

                    TestContext.Out.WriteLine(
                        $"{item.Login.PadRight(10)}" +
                        $"{item.FirstName.PadRight(20)}" +
                        $"{item.Department.PadRight(20)}" +
                        $"{item.Email.PadRight(40)}" +
                        $"{item.Role.PadRight(20)}"
                    );

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
        public async Task GetAsync_CheckLogin(Users user)
        {
            var userTest = await _usersRepository.GetAsync(user.Login);

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

            Assert.That(user.Id, Is.EqualTo(user.Id), "ERROR - Id is not equal");
            Assert.That(user.Login, Is.EqualTo(user.Login), "ERROR - Login is not equal");
            Assert.That(user.FirstName, Is.EqualTo(user.FirstName), "ERROR - FirstName is not equal");
            Assert.That(user.LastName, Is.EqualTo(user.LastName), "ERROR - LastName is not equal");
            Assert.That(user.Department, Is.EqualTo(user.Department), "ERROR - Department is not equal");
            Assert.That(user.Email, Is.EqualTo(user.Email), "ERROR - Email is not equal");
            Assert.That(user.Phone, Is.EqualTo(user.Phone), "ERROR - Phone is not equal");
            Assert.That(user.Role, Is.EqualTo(user.Role), "ERROR - Role is not equal");
            Assert.That(user.PasswordHash, Is.EqualTo(user.PasswordHash), "ERROR - PasswordHash is not equal");
        }

        [TestCaseSource(typeof(UsersRepositoryTestsData), nameof(UsersRepositoryTestsData.CRUDCases))]
        public async Task CRUDAsync(Users user)
        {
            user.Id = Guid.NewGuid();
            await _usersRepository.CreateAsync(user);

            var testUser = await _usersRepository.GetAsync(user.Id);

            Assert.That(testUser, Is.TypeOf<Users>(), "ERROR - return type");

            TestContext.Out.WriteLine($"Id         : {testUser.Id}");
            TestContext.Out.WriteLine($"Login      : {testUser.Login}");
            TestContext.Out.WriteLine($"FirstName  : {testUser.FirstName}");
            TestContext.Out.WriteLine($"LastName   : {testUser.LastName}");
            TestContext.Out.WriteLine($"Department : {testUser.Department}");
            TestContext.Out.WriteLine($"Phone      : {testUser.Email}");
            TestContext.Out.WriteLine($"Phone      : {testUser.Phone}");
            TestContext.Out.WriteLine($"Phone      : {testUser.Role}");
            TestContext.Out.WriteLine($"Phone      : {testUser.PasswordHash}");

            Assert.That(testUser.Id, Is.EqualTo(user.Id), "ERROR - Id is not equal");
            Assert.That(testUser.Login, Is.EqualTo(user.Login), "ERROR - Login is not equal");
            Assert.That(testUser.FirstName, Is.EqualTo(user.FirstName), "ERROR - FirstName is not equal");
            Assert.That(testUser.LastName, Is.EqualTo(user.LastName), "ERROR - LastName is not equal");
            Assert.That(testUser.Department, Is.EqualTo(user.Department), "ERROR - Department is not equal");
            Assert.That(testUser.Email, Is.EqualTo(user.Email), "ERROR - Email is not equal");
            Assert.That(testUser.Phone, Is.EqualTo(user.Phone), "ERROR - Phone is not equal");
            Assert.That(testUser.Role, Is.EqualTo(user.Role), "ERROR - Role is not equal");
            Assert.That(testUser.PasswordHash, Is.EqualTo(user.PasswordHash), "ERROR - PasswordHash is not equal");

            await _usersRepository.DeleteAsync(testUser.Id);
            testUser = await _usersRepository.GetAsync(testUser.Id);

            if (testUser is not null) Assert.Fail("ERROR - delete user");
        }
    }
}