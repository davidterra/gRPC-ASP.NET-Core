using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApp.Mvc.Models;
using WebApp.Mvc.Services.gRPC;

namespace WebApp.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWeatherForecastGrpcService _weatherForecastGrpcService;

        private readonly HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger,
                              IWeatherForecastGrpcService weatherForecastGrpcService,
                              HttpClient httpClient,
                              IOptions<AppSettings> settings)
        {
            _weatherForecastGrpcService = weatherForecastGrpcService;

            httpClient.BaseAddress = new Uri(settings.Value.AuthUrl);
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await DummyLogin();
            return View();
        }

        [HttpGet("forecasts")]
        public async Task<IActionResult> GetForecasts()
        {
            return Ok(await _weatherForecastGrpcService.GetForecasts());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login()
        {
            // Exemplo usando Autorize com gRPC.

            await DummyLogin();

            return Ok();

        }

        private async Task DummyLogin()
        {
            var user = new { Email = "test@test.it", Password = "Test@123" };

            var content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/login", content);

            response.EnsureSuccessStatusCode();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60),
                IsPersistent = true
            };

            var data = JsonSerializer.Deserialize<UserResponseLogin>(await response.Content.ReadAsStringAsync(), options);

            var jwtSecToken = new JwtSecurityTokenHandler().ReadToken(data.AccessToken) as JwtSecurityToken;

            var claims = new List<Claim> { new Claim("JWT", data.AccessToken) };
            claims.AddRange(jwtSecToken.Claims);

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
