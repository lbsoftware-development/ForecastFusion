using ForecastFusion.Api;
using ForecastFusion.Api.Caching;
using ForecastFusion.Application.Caching;
using ForecastFusion.Application.Contracts;
using ForecastFusion.Application.Interactors;
using ForecastFusion.Application.Services;
using ForecastFusion.Infrastructure.Repositories;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using Serilog.Events;
using DomainEntities = ForecastFusion.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<WeatherForecastUseCase>();
builder.Services.AddScoped<UserProfileUseCase>();
builder.Services.AddScoped<IWeatherForecastRepository, WeatherForecastRepository>();
builder.Services.AddScoped<IUserProfileRespository, UserProfileRepository>();
builder.Services.AddScoped<IAzureKeyVaultService, AzureKeyVaultService>();
builder.Services.AddSingleton<ICacheService, InMemoryCacheService>();

builder.Services.AddScoped<IAzureTableStorageService>(provider =>
{
    var keyVaultService = provider.GetService<IAzureKeyVaultService>();
    string tableStorageConnectionString = keyVaultService!.GetSecretFromVault("forecastfusiondevtablestorageconnstring").Result;
    return new AzureTableStorageService(tableStorageConnectionString);
});

builder.Services.AddMemoryCache();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseIdempotencyMiddleware();

Log.Logger = new LoggerConfiguration()
                                    .MinimumLevel.Debug()
                                    .WriteTo.Console()
                                    .WriteTo.File("logs/forecast-fusion-logs-.txt", LogEventLevel.Verbose, retainedFileCountLimit: 7, rollingInterval: RollingInterval.Day)
                                    .CreateLogger();



using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;

    var weatherForecastUseCase = services.GetRequiredService<WeatherForecastUseCase>();
    var userProfileRepoUserCase = services.GetRequiredService<UserProfileUseCase>();
    var logger = app.Logger;
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
            logger.LogDebug("GET UserProfile no Country Provided");
            return Results.BadRequest("Country cannot be empty");
        }

        if (String.IsNullOrEmpty(userId))
        {
            logger.LogDebug("GET UserProfile no UserId Provided");
            return Results.BadRequest("User ID cannot be empty");
        }

        logger.LogInformation("GET UserProfile for Country: {country} and UserId: {userId}", Country, userId);

        var userProfileEntity = await userProfileRepoUserCase.GetUserProfileAsync(Country, userId);
        
        if (!userProfileEntity.IsSuccess)
        {
            logger.LogError("GET UserProfile not successful, responsecode: {responseCode}, Exception: {@exception}",
                userProfileEntity.HttpStatusCode, userProfileEntity.Error);
            return Results.StatusCode((int)userProfileEntity.HttpStatusCode!);
        }

        logger.LogInformation("GET UserProfile successful for country: {country} & userId: {userId}", Country, userId);
        return Results.Ok(userProfileEntity.Value);
    })
        .WithName("GetUserProfile")
        .WithOpenApi();

    app.MapPut("/UserProfile", async (DomainEntities.UserProfile userProfile) =>
    {
        if (userProfile == null)
        {
            logger.LogError("PUT UserProfile no/invalid user profile");
            return Results.BadRequest("No/invalid user profile provided");
        }
        var result = await userProfileRepoUserCase.UpsertUserProfileAsync(userProfile);
        logger.LogInformation("PUT UserProfile result: {statusCode}", result.HttpStatusCode.ToString());
        return Results.StatusCode((int)result.HttpStatusCode!);
    });
}


app.Run();


