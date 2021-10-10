using Microsoft.AspNetCore.Mvc.Testing;
using mini_ITS.Web.Models.UsersController;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace mini_ITS.Web.Tests2
{
    public class IntegrationTest
    {
        protected readonly HttpClient TestClient;
        //private IConfiguration Configuration { get; }

        protected IntegrationTest()
        {
            //using Microsoft.AspNetCore.Mvc.Testing;
            var appFactory = new WebApplicationFactory<Startup>();
            TestClient = appFactory.CreateClient();
        }

        protected async Task LoginAsync()
        {
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Users.Login, new LoginData
            {
                Login = "admin",
                Password = "admin"
            });

            var test = response;
        }

        protected async Task<PostResponse> CreatePostAsync()
        {
            var person = new LoginData {
                Login = "admin",
                Password = "admin"
            };

            var json = JsonConvert.SerializeObject(person);
            var request = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Users.GetAll, request);

            return JsonConvert.DeserializeObject<PostResponse>(
                await response.Content.ReadAsStringAsync()
                );
        }
    }
}

public class PostResponse
{
    public string Login;
    public string firstName;
    public string lastName;
    public string department;
    public string role;
    public bool isLogged;
}