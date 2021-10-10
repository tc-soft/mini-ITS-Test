using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace mini_ITS.Web.Tests2
{
    public class UnitTest1 : IntegrationTest
    {
        [Fact]
        public async Task Test_XUnit()
        {
            //Arrange
            //await LoginAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.WeatherForecast.Get);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Test_XUnit1()
        {
            //Arrange
            //await LoginAsync();
            var loginData = new LoginData
            {
                Login = "admin",
                Password = "admin"
            };

            var content = JsonConvert.SerializeObject(loginData);
            var stringContent = new StringContent(content, Encoding.UTF8, "application/json");

            //Act
            var response = await TestClient.PostAsync(ApiRoutes.Users.Login, stringContent);
            //var response = await TestClient.GetAsync(ApiRoutes.Users.LoginStatus);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
