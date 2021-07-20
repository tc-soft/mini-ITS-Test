using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using mini_ITS.Core.Database;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Models;
using mini_ITS.Core.Options;
using mini_ITS.Core.Repository;
using mini_ITS.Core.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mini_ITS.Core.Tests.Services
{
    [TestFixture]
    public class UsersServicesTests
    {
        private IOptions<DatabaseOptions> _databaseOptions;
        private ISqlConnectionString _sqlConnectionString;

        private IMapper _mapper;
        private IPasswordHasher<Users> _hasher;
        private IUsersRepository _usersRepository;
        private IUsersService _usersService;

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
            _usersService = new UsersService(_usersRepository, _mapper, _hasher);
        }
        [Test]
        public async Task GetAsync_CheckAll()
        {
            var users = await _usersService.GetAsync();
            TestContext.Out.WriteLine($"Row's : {users.Count()}");

            Assert.That(users.Count() > 0, "ERROR - users is empty");
            Assert.That(users, Is.All.InstanceOf<UsersDto>(), "ERROR - each instance of <Users>()");
            Assert.That(users, Is.Ordered.Ascending.By("Login"), "ERROR - sort");
            Assert.That(users, Is.Unique);

            foreach (var item in users)
            {
                if (item.FirstName.Length >= 3 && item.LastName.Length >= 5)
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
        [Test]
        public async Task GetAsync_CheckDepartmentRole(
            [ValueSource(typeof(UsersServicesTestsData), nameof(UsersServicesTestsData.testDepartment))] string department,
            [ValueSource(typeof(UsersServicesTestsData), nameof(UsersServicesTestsData.testRole))] string role)
        {

            //department = "Sales";
            //role = "Manager";
            var users = await _usersService.GetAsync(department, role);
            TestContext.Out.WriteLine($"Row's : {users.Count()}");
            TestContext.Out.WriteLine($"{("Login").PadRight(10)}{("FirstName").PadRight(20)}{("Department").PadRight(20)}{("Email").PadRight(40)}{("Role").PadRight(20)}");

            Assert.That(users.Count() > 0, "ERROR - users is empty");
            Assert.That(users, Is.All.InstanceOf<UsersDto>(), "ERROR - each instance of <Users>()");
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
            [ValueSource(typeof(UsersServicesTestsData), nameof(UsersServicesTestsData.testDepartment))] string department,
            [ValueSource(typeof(UsersServicesTestsData), nameof(UsersServicesTestsData.testRole))] string role)
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

            var users = await _usersService.GetAsync(sqlQueryConditionList);
            TestContext.Out.WriteLine($"Row's : {users.Count()}");

            Assert.That(users.Count() > 0, "ERROR - users is empty");
            Assert.That(users, Is.All.InstanceOf<UsersDto>(), "ERROR - return Instance must be <Users>");
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
        [TestCaseSource(typeof(UsersServicesTestsData), nameof(UsersServicesTestsData.SqlPagedQueryCases))]
        public async Task GetAsync_CheckSqlPagedQuery(SqlPagedQuery<Users> sqlPagedQuery)
        {
            var usersList = await _usersService.GetAsync(sqlPagedQuery);

            for (int i = 1; i <= usersList.TotalPages; i++)
            {
                sqlPagedQuery.Page = i;
                var users = await _usersService.GetAsync(sqlPagedQuery);

                string filterString = null;
                sqlPagedQuery.Filter.ForEach(x =>
                {
                    if (x == sqlPagedQuery.Filter.First() || x == sqlPagedQuery.Filter.Last())
                        filterString += $", {x.Name}={x.Value}";
                    else
                        filterString += $" {x.Name}={x.Value}";
                });

                TestContext.Out.WriteLine($"\nPage {users.CurrentPage}/{usersList.TotalPages} - ResultsPerPage={users.ResultsPerPage}, TotalResults={users.TotalResults}{filterString}");

                TestContext.Out.WriteLine(
                    $"{"Login".PadRight(10)}" +
                    $"{"FirstName".PadRight(20)}" +
                    $"{"Department".PadRight(20)}" +
                    $"{"Email".PadRight(40)}" +
                    $"{"Role".PadRight(20)}"
                );

                Assert.That(users.Results.Count() > 0, "ERROR - users is empty");
                Assert.That(users.Results, Is.All.InstanceOf<UsersDto>(), "ERROR - All.InstanceOf<Users>()");
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
                    sqlPagedQuery.Filter.ForEach(x =>
                    {
                        if (x.Value is not null)
                        {
                            Assert.That(
                                item.GetType().GetProperty(x.Name).GetValue(item, null),
                                Is.EqualTo(x.Value),
                                $"ERROR - Filter {x.Name} is not equal");
                        }
                    });

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
        [Test]
        public async Task SetPasswordAsync()
        {
            var user = _usersService.GetAsync("yaveomic");
            await _usersService.SetPasswordAsync("atkincol", "NoweHasłozcaszc123$!@#");

            
            //var result = _hash.VerifyHashedPassword(userPass, pass);
            //Assert.That(result, Is.EqualTo(PasswordVerificationResult.Success));
        }
    }
}