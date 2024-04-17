using ForecastFusion.Application.Caching;
using ForecastFusion.Application.Contracts;
using ForecastFusion.Domain.Entities;

namespace ForecastFusion.Application.Interactors
{
    public class UserProfileUseCase(IUserProfileRespository userProfileRespository, ICacheService cacheService)
    {
        private static string USER_PROFILE_CACHE_KEY = "UserProfile";
        
        //private readonly IUserProfileRespository _userProfileRespository = userProfileRespository;
        //private readonly ICacheService _cacheService = cacheService;

        public async Task<Result<UserProfile>> GetUserProfileAsync(string country, string userId)
        {
            var cacheKey = $"{USER_PROFILE_CACHE_KEY}_{userId}";

            object result = cacheService.GetValue(cacheKey);

            if (result != null)
            {
                return Result<UserProfile>.Success((UserProfile)result);
            }

            return await userProfileRespository.GetUserProfileAsync(country, userId);
        }

        public async Task<Result> UpsertUserProfileAsync(UserProfile userProfile)
        {
            var cacheKey = $"{USER_PROFILE_CACHE_KEY}_{userProfile.Id}";
            object result = cacheService.GetValue(cacheKey);

            if (result != null)
            {
                cacheService.RemoveValue(cacheKey);
            }
            cacheService.SetValue(cacheKey, userProfile);
            return await userProfileRespository.UpsertUserProfileAsync(userProfile);
        }
    }
}
