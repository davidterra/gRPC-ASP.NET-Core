using System.Collections.Generic;
using System.Threading.Tasks;
using Weather.Api.Services.gRPC;
using WebApp.Mvc.Models;

namespace WebApp.Mvc.Services.gRPC
{
    public class WeatherForecastGrpcService : IWeatherForecastGrpcService
    {
        private readonly WeatherForecasts.WeatherForecastsClient _weatherForecastsClient;

        public WeatherForecastGrpcService(WeatherForecasts.WeatherForecastsClient weatherForecastsClient)
            => _weatherForecastsClient = weatherForecastsClient;

        public async Task<IEnumerable<WeatherForecastDto>> GetForecasts()
        {
            var response = await _weatherForecastsClient.GetWeatherAsync(new WeatherRequest());

            return MapProtoToDto(response);
        }

        private IEnumerable<WeatherForecastDto> MapProtoToDto(WeatherResponse response)
        {
            foreach (var item in response.Forecasts)
            {
                var dto = new WeatherForecastDto
                {
                    TemperatureC = item.Temperaturec,
                    TemperatureF = item.Temperaturef,
                    Summary = item.Summary,
                };

                dto.SetDate(item.Date.ToDateTimeOffset());

                yield return dto;
            }
        }
    }
}
