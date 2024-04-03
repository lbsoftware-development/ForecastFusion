using ForecastFusion.Application.Contracts;
using ForecastFusion.Application.Interactors;
using ForecastFusion.Application.Services;
using ForecastFusion.Domain.Entities;
using ForecastFusion.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using ForecastFusion.Infrastructure.Entities;

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
    var azureTableStorageService = services.GetRequiredService<IAzureTableStorageService>();

    app.MapGet("/weatherforecast", async () =>
    {
        var tableconnectionstring = await azureKeyVaultService.GetSecretFromVault("forecastfusiondevtablestorageconnstring");
        var userProfileEntity = await azureTableStorageService.RetrieveEntityAsync<ForecastFusion.Infrastructure.Entities.UserProfile>("UserProfile", "West Midlands", "6eeb7793-f220-497b-98b7-b5b60fce9707");
        var forecasts = weatherForecastUseCase.GetForecastsAsync().Result;
        return forecasts;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();
}


app.Run();


