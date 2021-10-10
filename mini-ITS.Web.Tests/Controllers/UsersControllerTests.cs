using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using mini_ITS.Core.Database;
using mini_ITS.Core.Models;
using mini_ITS.Web.Models.UsersController;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace mini_ITS.Web.Tests.Controllers
{
    public class UsersControllerTests : IntegrationTest
    {
        [Test]
        public async Task UsersController_LoginAsync()
        {
            await LoginAsync();
            var response = await TestClient.GetAsync(ApiRoutes.Users.LoginStatus);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            string responseJson = response.Content.ReadAsStringAsync().Result;
            var respondLogin = JsonConvert.DeserializeObject<RespondLogin>(responseJson);

            TestContext.Out.WriteLine($"Login      : {respondLogin.Login}");
            TestContext.Out.WriteLine($"FirstName  : {respondLogin.FirstName}");
            TestContext.Out.WriteLine($"LastName   : {respondLogin.LastName}");
            TestContext.Out.WriteLine($"Department : {respondLogin.Department}");
            TestContext.Out.WriteLine($"Role       : {respondLogin.Role}");
            TestContext.Out.WriteLine($"isLogged   : {respondLogin.isLogged}");
        }

        [Test]
        public async Task UsersController_LoginStatusAsync()
        {
            await LoginAsync();
            var response = await TestClient.GetAsync(ApiRoutes.Users.LoginStatus);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            string responseJson = response.Content.ReadAsStringAsync().Result;
            var respondLogin = JsonConvert.DeserializeObject<RespondLogin>(responseJson);

            TestContext.Out.WriteLine($"Login      : {respondLogin.Login}");
            TestContext.Out.WriteLine($"FirstName  : {respondLogin.FirstName}");
            TestContext.Out.WriteLine($"LastName   : {respondLogin.LastName}");
            TestContext.Out.WriteLine($"Department : {respondLogin.Department}");
            TestContext.Out.WriteLine($"Role       : {respondLogin.Role}");
            TestContext.Out.WriteLine($"isLogged   : {respondLogin.isLogged}");
        }

        [Test]
        public async Task UsersController_IndexAsync()
        {
            await LoginAsync();

            var sqlPagedQuery = new SqlPagedQuery<Users>
            {
                Filter = new List<SqlQueryCondition>()
                    {
                        new SqlQueryCondition
                        {
                            Name = "Department",
                            Operator = SqlQueryOperator.Equal,
                            Value = "Sales"
                        },
                        new SqlQueryCondition
                        {
                            Name = "Role",
                            Operator = SqlQueryOperator.Equal,
                            Value = null
                        }
                    },
                SortColumnName = "Login",
                SortDirection = "DESC",
                Page = 1,
                ResultsPerPage = 5
            };

            var json = JsonConvert.SerializeObject(sqlPagedQuery);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var encodedContent = new FormUrlEncodedContent(json);

            String encoded = URLEncoder.encode(sqlPagedQuery, StandardCharsets.UTF_8);

            var response = await TestClient.GetAsync(ApiRoutes.Users.Index, new FormUrlEncodedContent(json);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }

    public class RespondLogin
    {
        public string Login;
        public string FirstName;
        public string LastName;
        public string Department;
        public string Role;
        public bool isLogged;
    }
}
