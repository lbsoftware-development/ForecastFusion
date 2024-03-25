using ForecastFusionDomain.Entities;

namespace ForecastFusionDomain.Contracts
{
    public interface IWeatherForecastRepository
    {
        Task<IEnumerable<WeatherForecast>> GetForecastsAsync();
    }
}
