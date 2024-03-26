using ForecastFusion.Application.Contracts;
using ForecastFusion.Domain.Entities;

namespace ForecastFusion.Infrastructure.Repositories
{
    public class WeatherForecastRepository : IWeatherForecastRepository
    {
        public Task<IEnumerable<WeatherForecast>> GetForecastsAsync()
        {
           var forecasts = new List<WeatherForecast>();
            forecasts.Add(new WeatherForecast(DateOnly.FromDateTime(DateTime.Now.AddDays(-5)), Random.Shared.Next(-20, 50), Weather.Summaries[0]));
            forecasts.Add(new WeatherForecast(DateOnly.FromDateTime(DateTime.Now.AddDays(-4)), Random.Shared.Next(-20, 50), Weather.Summaries[1]));
            forecasts.Add(new WeatherForecast(DateOnly.FromDateTime(DateTime.Now.AddDays(-3)), Random.Shared.Next(-20, 50), Weather.Summaries[2]));
            forecasts.Add(new WeatherForecast(DateOnly.FromDateTime(DateTime.Now.AddDays(-2)), Random.Shared.Next(-20, 50), Weather.Summaries[3]));
            forecasts.Add(new WeatherForecast(DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), Random.Shared.Next(-20, 50), Weather.Summaries[4]));
            return Task.FromResult<IEnumerable<WeatherForecast>>(forecasts);
        }
    }
}
