using ForecastFusion.Application.Contracts;
using ForecastFusion.Domain.Entities;

namespace ForecastFusion.Application.Interactors
{
    public class UserProfileUseCase(IUserProfileRespository userProfileRespository)
    {
        private readonly IUserProfileRespository _userProfileRespository = userProfileRespository;

        public async Task<Result<UserProfile>> GetUserProfileAsync(string country, string userId)
        {
            return await _userProfileRespository.GetUserProfileAsync(country, userId);
        }

        public async Task<Result> UpsertUserProfileAsync(UserProfile userProfile)
        {
            return await _userProfileRespository.UpsertUserProfileAsync(userProfile);
        }
    }
}
