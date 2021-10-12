using mini_ITS.Core.Database;
using mini_ITS.Core.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
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

            //var response = responseStatusCode.Content.ReadAsStreamAsync().Result;
            //var response = responseStatusCode.Content.ReadAsStringAsync().Result;

            var result = await response.Content.ReadAsStringAsync();

            //var respondLogin = await response.Content.ReadFromJsonAsync<RespondLogin>();
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            var respondLogin = JsonSerializer.Deserialize<RespondLogin>(result, options);

            //var respondLogin = JsonConvert.DeserializeObject<RespondLogin>(response);
            //var respondLogin = await JsonSerializer.DeserializeAsync<RespondLogin>(result);

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
            var responseStatusCode = await TestClient.GetAsync(ApiRoutes.Users.LoginStatus);
            Assert.That(responseStatusCode.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            //var response = await TestClient.GetStringAsync(ApiRoutes.Users.LoginStatus);

            var responseJson = await responseStatusCode.Content.ReadAsStreamAsync();
            //var respondLogin = JsonConvert.DeserializeObject<RespondLogin>(response);
            //var respondLogin = await JsonSerializer.DeserializeAsync<RespondLogin>(responseJson);

            //TestContext.Out.WriteLine($"Login      : {respondLogin.Login}");
            //TestContext.Out.WriteLine($"FirstName  : {respondLogin.FirstName}");
            //TestContext.Out.WriteLine($"LastName   : {respondLogin.LastName}");
            //TestContext.Out.WriteLine($"Department : {respondLogin.Department}");
            //TestContext.Out.WriteLine($"Role       : {respondLogin.Role}");
            //TestContext.Out.WriteLine($"isLogged   : {respondLogin.isLogged}");
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

            sqlPagedQuery.Filter
                .Select((filter, index) => (filter, index)).ToList()
                .ForEach((x) =>
                {
                    queryParameters.Add($"Filter[{x.index}].Name", x.filter.Name);
                    queryParameters.Add($"Filter[{x.index}].Operator", x.filter.Operator);
                    queryParameters.Add($"Filter[{x.index}].Value", x.filter.Value);
                });

            queryParameters.Add("SortColumnName", "Login");
            queryParameters.Add("SortDirection", "DESC");
            queryParameters.Add("Page", "1");
            queryParameters.Add("ResultsPerPage", "5");

            var queryString = new FormUrlEncodedContent(queryParameters).ReadAsStringAsync();

            var responseStatus = await TestClient.GetAsync($"{ApiRoutes.Users.Index}?{queryString.Result}");

            var response = TestClient.GetStreamAsync($"{ApiRoutes.Users.Index}?{queryString.Result}");
            
            //var repositories = await JsonSerializer.DeserializeAsync<SqlPagedResult<UsersDto>>(await response);
                        
            Assert.That(responseStatus.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        }
    }

    public class RespondLogin
    {
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
        public string Role { get; set; }
        public bool isLogged { get; set; }
    }
}
