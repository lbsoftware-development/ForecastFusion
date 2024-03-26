using ForecastFusion.Domain.Entities;

namespace ForecastFusion.Application.Contracts
{
    public interface IWeatherForecastRepository
    {
        Task<IEnumerable<WeatherForecast>> GetForecastsAsync();
    }
}
