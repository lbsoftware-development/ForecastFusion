using ForecastFusion.Api;
using ForecastFusion.Application.Contracts;
using ForecastFusion.Application.Interactors;
using ForecastFusion.Application.Services;
using ForecastFusion.Infrastructure.Repositories;
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
app.UseIdempotencyMiddleware();

using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;

    var weatherForecastUseCase = services.GetRequiredService<WeatherForecastUseCase>();
    var userProfileRepoUserCase = services.GetRequiredService<UserProfileUseCase>();

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
            return Results.BadRequest("Country cannot be empty");
        }

        if (String.IsNullOrEmpty(userId))
        {
            return Results.BadRequest("User ID cannot be empty");
        }

        var userProfileEntity = await userProfileRepoUserCase.GetUserProfileAsync(Country, userId);
        
        if (!userProfileEntity.IsSuccess)
        {
            return Results.StatusCode((int)userProfileEntity.HttpStatusCode!);
        }
        
        return Results.Ok(userProfileEntity.Value);
    })
        .WithName("GetUserProfile")
        .WithOpenApi();

    app.MapPut("/UserProfile", async (DomainEntities.UserProfile userProfile) =>
    {
        var result = await userProfileRepoUserCase.UpsertUserProfileAsync(userProfile);
        return Results.StatusCode((int)result.HttpStatusCode!);
    });
}


app.Run();


