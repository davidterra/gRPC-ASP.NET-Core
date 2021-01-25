using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp.Mvc.Models;

namespace WebApp.Mvc.Services.gRPC
{
    public interface IWeatherForecastGrpcService
    {
        Task<IEnumerable<WeatherForecastDto>> GetForecasts();
    }
}