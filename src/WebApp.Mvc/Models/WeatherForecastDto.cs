using System;

namespace WebApp.Mvc.Models
{
    public class WeatherForecastDto
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF { get; set; }

        public string Summary { get; set; }

        public void SetDate(DateTimeOffset date) => Date = date.UtcDateTime;
    }
}
