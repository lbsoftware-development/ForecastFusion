using ForecastFusion.Application.Contracts;
using ForecastFusion.Domain.Entities;

namespace ForecastFusion.Application.Interactors
{
    public class WeatherForecastUseCase(IWeatherForecastRepository repository)
    {
        private readonly IWeatherForecastRepository _repository = repository;

        public async Task<IEnumerable<WeatherForecast>> GetForecastsAsync()
        {
            // Retrieve forecasts from the repository
            return await _repository.GetForecastsAsync();
        }
    }
}
