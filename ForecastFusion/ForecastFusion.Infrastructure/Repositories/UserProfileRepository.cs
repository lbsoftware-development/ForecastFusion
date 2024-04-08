using Azure.Data.Tables;
using ForecastFusion.Application;
using ForecastFusion.Application.Contracts;
using ForecastFusion.Application.DTOs;
using ForecastFusion.Domain.Entities;
using ForecastFusion.Infrastructure.Entities;
using ForecastFusion.Infrastructure.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForecastFusion.Infrastructure.Repositories
{
    internal class UserProfileRepository : IUserProfileRespository
    {
        private const string USER_PROFILE_TABLENAME = "UserProfile";
        private readonly IAzureTableStorageService _azureTableStorageService;
        public UserProfileRepository(IAzureTableStorageService azureTableStorageService)
        {
            _azureTableStorageService = azureTableStorageService;
        }

        public async Task<Result<UserProfileDto>> GetUserProfileAsync(string country, string userId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(country, nameof(country));
            ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));
            var userProfileEntity = await _azureTableStorageService.RetrieveEntityAsync<Entities.UserProfile>(USER_PROFILE_TABLENAME, country, userId);
            if (userProfileEntity.IsSuccess)
            {
                var dto = UserProfileMapper.InfraToDto((Entities.UserProfile)userProfileEntity.Value);
                return Result<UserProfileDto>.Success(dto);
            }

            return Result.Failure()
        }

        public Task<Result> UpsertUserProfile(UserProfileDto userProfile)
        {
            ArgumentNullException.ThrowIfNull(userProfile, nameof(userProfile));
            return _azureTableStorageService.UpsertEntityAsync(userProfile, USER_PROFILE_TABLENAME);
        }
    }
}
