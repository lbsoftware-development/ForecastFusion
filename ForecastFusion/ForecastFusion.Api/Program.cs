using Asp.Versioning;
using ForecastFusion.Api.Caching;
using ForecastFusion.Application.Caching;
using ForecastFusion.Application.Contracts;
using ForecastFusion.Application.Interactors;
using ForecastFusion.Application.Services;
using ForecastFusion.Infrastructure.Repositories;
using Serilog;
using Serilog.Events;
using DomainEntities = ForecastFusion.Domain.Entities;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using ForecastFusion.Api.Middleware;

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
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new HeaderApiVersionReader("api-version");
    options.ReportApiVersions = true;
});

//Add Rate Limiting
builder.Services.AddRateLimiter(_ =>
{
    _.AddFixedWindowLimiter("FixedRatePolicy", options =>
    {
        options.PermitLimit = 1;
        options.Window = TimeSpan.FromSeconds(30);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2;
    });
});

//Add logging using SeriLog
Log.Logger = new LoggerConfiguration()
                                    .MinimumLevel.Debug()
                                    .WriteTo.Console()
                                    .WriteTo.File("logs/forecast-fusion-logs-.txt", LogEventLevel.Verbose, retainedFileCountLimit: 7, rollingInterval: RollingInterval.Day)
                                    .CreateLogger();

// Add Serilog as a provider for the logging framework
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders(); // Clear existing logging providers
    loggingBuilder.AddSerilog(dispose: true); // Add Serilog as the logging provider
});

builder.Services.AddSingleton(Log.Logger);

var app = builder.Build();

var versionSet = app.NewApiVersionSet()
    .HasApiVersion(new Asp.Versioning.ApiVersion(1))
    .HasApiVersion(new Asp.Versioning.ApiVersion(2))
    .ReportApiVersions()
    .Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(o =>
    {
        //o.RoutePrefix = "";
        o.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
        o.SwaggerEndpoint("/swagger/v2/swagger.json", "V2 Docs");
    });

}

//Add Rate Limiting
app.UseRateLimiter();

//Add global exception handling
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseRouting();
app.UseIdempotencyMiddleware();





using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;

    var weatherForecastUseCase = services.GetRequiredService<WeatherForecastUseCase>();
    var userProfileRepoUserCase = services.GetRequiredService<UserProfileUseCase>();
    var logger = app.Logger;
    app.MapGet("/weatherforecast", async () =>
    {
        throw new Exception("Test error");
        var forecasts = await weatherForecastUseCase.GetForecastsAsync();
        return forecasts;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi()
    .RequireRateLimiting("FixedRatePolicy");

    app.MapGet("/UserProfile/{Country}/{userId}", async (string Country, string userId, HttpContext context) =>
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

        var apiVersion = context.GetRequestedApiVersion();

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
        .WithName("GetUserProfile V1")
        .WithOpenApi()
        .WithApiVersionSet(versionSet)
        .MapToApiVersion(1);


    app.MapGet("/UserProfile/{Country}/{userId}", (string Country, string userId) =>
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

        return Results.Ok("V2 Ok");
    })
        .WithName("GetUserProfile V2")
        .WithOpenApi()
        .WithApiVersionSet(versionSet)
        .MapToApiVersion(2);

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

