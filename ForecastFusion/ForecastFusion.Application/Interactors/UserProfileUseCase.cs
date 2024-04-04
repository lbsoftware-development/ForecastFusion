using ForecastFusion.Application.Contracts;
using ForecastFusion.Application.DTOs;

namespace ForecastFusion.Application.Interactors
{
    public class UserProfileUseCase(IUserProfileRespository userProfileRespository)
    {
        private readonly IUserProfileRespository _userProfileRespository = userProfileRespository;

        public async Task<Result<UserProfileDto>> GetUserProfile(string country, string userId)
        {
            return await _userProfileRespository.GetUserProfile(country, userId);
        }

        public async Task<Result> UpsertUserProfile(UserProfileDto userProfile)
        {
            return await _userProfileRespository.UpsertUserProfile(userProfile);
        }
    }
}
