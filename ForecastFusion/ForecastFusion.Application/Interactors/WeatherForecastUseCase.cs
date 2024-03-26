using ForecastFusion.Application.Contracts;
using ForecastFusion.Domain.Entities;

namespace ForecastFusion.Application.Interactors
{
    public class WeatherForecastUseCase
    {
        private readonly IWeatherForecastRepository _repository;

        public WeatherForecastUseCase(IWeatherForecastRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<WeatherForecast>> GetForecastsAsync()
        {
            // Retrieve forecasts from the repository
            return await _repository.GetForecastsAsync();
        }
    }
}
