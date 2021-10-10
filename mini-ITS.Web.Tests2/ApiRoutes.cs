using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mini_ITS.Web.Tests2
{
    public static class ApiRoutes
    {
        private static readonly string _baseUrl = "https://localhost:44375/";

        public static class Users
        {
            private static readonly string _usersControllerUrl = string.Concat(_baseUrl, "Users");

            public static readonly string GetAll = _usersControllerUrl;
            public static readonly string Login = string.Concat(_usersControllerUrl, "/Login");
            public static readonly string LoginStatus = string.Concat(_usersControllerUrl, "/LoginStatus");
            public static readonly string Delete = string.Concat(_usersControllerUrl, "/{userId}");
        }

        public static class WeatherForecast
        {
            private static readonly string _weatherForecastControllerUrl = string.Concat(_baseUrl, "WeatherForecast");

            public static readonly string Get = _weatherForecastControllerUrl;
        }
    }
}