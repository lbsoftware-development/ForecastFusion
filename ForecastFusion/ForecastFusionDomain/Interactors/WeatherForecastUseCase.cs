using ForecastFusionDomain.Contracts;
using ForecastFusionDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForecastFusionDomain.Interactors
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
