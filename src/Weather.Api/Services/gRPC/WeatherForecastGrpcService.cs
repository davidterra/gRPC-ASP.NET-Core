using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Google.Protobuf.WellKnownTypes;

namespace Weather.Api.Services.gRPC
{
    [Authorize]
    public class WeatherForecastGrpcService : WeatherForecasts.WeatherForecastsBase
    {

        private readonly ILogger<WeatherForecastGrpcService> _logger;
        public WeatherForecastGrpcService(ILogger<WeatherForecastGrpcService> logger)
        {
            _logger = logger;
        }

        public override  Task<WeatherResponse> GetWeather(WeatherRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Call GetWeather");

            return Task.FromResult(MapToProtoResponse(DummyForecasts()));

        }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private IEnumerable<WeatherForecast> DummyForecasts()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }


        private static WeatherResponse MapToProtoResponse(IEnumerable<WeatherForecast> forecasts)
        {
            var proto = new WeatherResponse();

            foreach (var item in forecasts)
            {
                proto.Forecasts.Add(new WeatherForecastReponse
                {
                    Date = Timestamp.FromDateTimeOffset(item.Date),
                    Temperaturec = item.TemperatureC,
                    Temperaturef = item.TemperatureF,
                    Summary = item.Summary,
                });
            }

            return proto;
        }
    }
}
