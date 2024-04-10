using ForecastFusion.Application;
using ForecastFusion.Application.Contracts;
using ForecastFusion.Infrastructure.Const;
using ForecastFusion.Infrastructure.Entities;
using ForecastFusion.Infrastructure.Mappings;
using DomainEntities = ForecastFusion.Domain.Entities;

namespace ForecastFusion.Infrastructure.Repositories
{
    public class UserProfileRepository : IUserProfileRespository
    {        
        private readonly IAzureTableStorageService _azureTableStorageService;
        public UserProfileRepository(IAzureTableStorageService azureTableStorageService)
        {
            _azureTableStorageService = azureTableStorageService;
        }

        public async Task<Result<DomainEntities.UserProfile>> GetUserProfileAsync(string country, string userId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(country, nameof(country));
            ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));
            var userProfileEntity = await _azureTableStorageService.RetrieveEntityAsync<UserProfile>(AzureStorageTableNames.USER_PROFILE, country, userId);
            if (userProfileEntity.IsSuccess)
            {
                var userProfile = UserProfileMapper.MapToDomain((UserProfile)userProfileEntity.Value);
                return Result<DomainEntities.UserProfile>.Success(userProfile);
            }

            return Result<DomainEntities.UserProfile>.Failure(default(DomainEntities.UserProfile)!, userProfileEntity.Error, (System.Net.HttpStatusCode)userProfileEntity.HttpStatusCode!);
        }

        public Task<Result> UpsertUserProfileAsync(DomainEntities.UserProfile userProfile)
        {
            ArgumentNullException.ThrowIfNull(userProfile, nameof(userProfile));
            var userProfileTableEntity = UserProfileMapper.MapToInfra(userProfile);
            return _azureTableStorageService.UpsertEntityAsync(userProfileTableEntity, AzureStorageTableNames.USER_PROFILE);
        }
    }
}
