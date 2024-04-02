using ForecastFusion.Application.Contracts;
using ForecastFusion.Application.Interactors;
using ForecastFusion.Application.Services;
using ForecastFusion.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<WeatherForecastUseCase>();
builder.Services.AddScoped<IWeatherForecastRepository, WeatherForecastRepository>();
builder.Services.AddScoped<IAzureKeyVaultService, AzureKeyVaultService>();
builder.Services.AddScoped<IAzureTableStorageService>(provider =>
{
    var keyVaultService = provider.GetService<IAzureKeyVaultService>();
    string tableStorageConnectionString = keyVaultService.GetSecretFromVault("forecastfusiondevtablestorageconnstring").Result;
    return new AzureTableStorageService(tableStorageConnectionString);
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;

    var weatherForecastUseCase = services.GetRequiredService<WeatherForecastUseCase>();
    var azureKeyVaultService = services.GetRequiredService<IAzureKeyVaultService>();

    app.MapGet("/weatherforecast", async () =>
    {
        var tableconnectionstring = await azureKeyVaultService.GetSecretFromVault("forecastfusiondevtablestorageconnstring");
        var forecasts = weatherForecastUseCase.GetForecastsAsync().Result;
        return forecasts;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();
}


app.Run();


