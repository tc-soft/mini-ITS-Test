using Microsoft.AspNetCore.Mvc.Testing;
using mini_ITS.Web.Models.UsersController;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace mini_ITS.Web.Tests
{
    public class IntegrationTest
    {
        protected readonly HttpClient TestClient;
        //private MyHttpClientSetup _setup;

        protected IntegrationTest()
        {
            //using Microsoft.AspNetCore.Mvc.Testing;
            var appFactory = new WebApplicationFactory<Startup>();
            TestClient = appFactory.CreateClient();
        }

        protected async Task<HttpResponseMessage> LoginAsync()
        {
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Users.Login, new LoginData
            {
                Login = "admin",
                Password = "admin"
            });

            return response;
        }

        protected async Task<HttpResponseMessage> LoginAsync1()
        {
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Users.Login, new LoginData
            {
                Login = "admin",
                Password = "admin1"
            });

            return response;
        }
    }
}
