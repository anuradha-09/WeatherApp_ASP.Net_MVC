using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherAppMVC.Models;

namespace WeatherAppMVC.Controllers
{
    public class WeatherController : Controller
    {
        private readonly HttpClient _httpClient;

        public WeatherController()
        {
            _httpClient = new HttpClient();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string cityName)
        {
            if (string.IsNullOrEmpty(cityName))
                return View();

            string apiKey = "ce3d69c489bdc9b247024d75e1cf03be"; // Replace with your OpenWeatherMap API key
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={apiKey}&units=metric";

            try
            {
                var response = await _httpClient.GetStringAsync(url);
                var data = JObject.Parse(response);

                var weather = new Weather
                {
                    City = data["name"].ToString(),
                    Country = data["sys"]["country"].ToString(),
                    Temperature = data["main"]["temp"].ToString(),
                    Humidity = data["main"]["humidity"].ToString(),
                    Pressure = data["main"]["pressure"].ToString(),
                    WeatherCondition = data["weather"][0]["description"].ToString(),
                    WindSpeed = data["wind"]["speed"].ToString()
                };

                // --- City Time & Timezone ---
                int timezoneOffsetSeconds = int.Parse(data["timezone"].ToString()); // seconds from UTC
                DateTime utcTime = DateTime.UtcNow;
                DateTime cityLocalTime = utcTime.AddSeconds(timezoneOffsetSeconds);

                weather.CityTime = cityLocalTime.ToString("hh:mm:tt");
                weather.Timezone = $"UTC{(timezoneOffsetSeconds >= 0 ? "+" : "-")}{timezoneOffsetSeconds / 3600}";

                return View(weather);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "City not found or API error.";
                return View();
            }
        }
    }
}
