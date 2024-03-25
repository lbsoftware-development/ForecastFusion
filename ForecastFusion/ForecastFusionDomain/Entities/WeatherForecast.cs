namespace ForecastFusionDomain.Entities
{
    public class WeatherForecast
    {
        DateOnly Date { get; set; }
        int TemperatureC { get; set; }

        public string? Summary { get; set; }
    }

    //internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    //{
    //    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    //}

}
