using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using mini_ITS.Core.Database;
using mini_ITS.Core.Models;

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


            var queryParameters = new Dictionary<string, string>();

            queryParameters.Add("SortColumnName", "Login");
            queryParameters.Add("SortDirection", "DESC");
            queryParameters.Add("Page", "1");
            queryParameters.Add("ResultsPerPage", "5");

            sqlPagedQuery.Filter
                .Select((filter, index) => (filter, index)).ToList()
                .ForEach((x) =>
                {
                    queryParameters.Add($"Filter[{x.index}].Name", x.filter.Name);
                    queryParameters.Add($"Filter[{x.index}].Operator", x.filter.Operator);
                    queryParameters.Add($"Filter[{x.index}].Value", x.filter.Value);
                });

            
            foreach (var item in sqlPagedQuery.Filter.Select((filter, index) => (filter, index)))
            {
                queryParameters.Add($"Filter[{item.index}].Name", item.filter.Name);
                queryParameters.Add($"Filter[{item.index}].Operator", item.filter.Operator);
                queryParameters.Add($"Filter[{item.index}].Value", item.filter.Value);
            }

            var queryString = new FormUrlEncodedContent(queryParameters).ReadAsStringAsync();
            //var queryString = await dictFormUrlEncoded.ReadAsStringAsync();

            var response = await TestClient.GetAsync($"{ApiRoutes.Users.Index}?{queryString}");

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
