using ForecastFusion.Application.Contracts;
using ForecastFusion.Application.Interactors;
using ForecastFusion.Application.Services;
using ForecastFusion.Domain.Entities;
using ForecastFusion.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using ForecastFusion.Infrastructure.Entities;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using ForecastFusion.Application.DTOs;
using ForecastFusion.Application.Mappings;

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
    string tableStorageConnectionString = keyVaultService!.GetSecretFromVault("forecastfusiondevtablestorageconnstring").Result;
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
    var azureTableStorageService = services.GetRequiredService<IAzureTableStorageService>();

    app.MapGet("/weatherforecast", async () =>
    {        
        var forecasts = await weatherForecastUseCase.GetForecastsAsync();
        return forecasts;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

    app.MapGet("/UserProfile/{Country}/{userId}", async (string Country, string userId) =>
    {
        if (String.IsNullOrEmpty(Country))
        {
            return Results.BadRequest("Country cannout be empty");
        }

        if (String.IsNullOrEmpty(userId))
        {
            return Results.BadRequest("User ID cannot be empty");
        }

        var userProfileEntity = await azureTableStorageService.RetrieveEntityAsync<ForecastFusion.Infrastructure.Entities.UserProfile>("UserProfile", Country, userId);
        
        if (!userProfileEntity.IsSuccess)
        {
            return Results.StatusCode((int)userProfileEntity.HttpStatusCode!);
        }
        
        return Results.Ok(userProfileEntity.Value);
    })
        .WithName("GetUserProfile")
        .WithOpenApi();

    app.MapPut("/UserProfile", async (UserProfileDto userProfile) =>
    {
        var result = await azureTableStorageService.UpsertEntityAsync(UserProfileDto, )
    });
}


app.Run();


