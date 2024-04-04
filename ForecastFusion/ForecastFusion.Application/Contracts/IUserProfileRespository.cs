using ForecastFusion.Application.DTOs;

namespace ForecastFusion.Application.Contracts
{
    public interface IUserProfileRespository
    {
        Task<Result<UserProfileDto>> GetUserProfile(string country, string userId);

        Task<Result> UpsertUserProfile(UserProfileDto userProfile);
    }
}
