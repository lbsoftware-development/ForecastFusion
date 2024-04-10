using ForecastFusion.Domain.Entities;

namespace ForecastFusion.Application.Contracts
{
    public interface IUserProfileRespository
    {
        Task<Result<UserProfile>> GetUserProfileAsync(string country, string userId);

        Task<Result> UpsertUserProfileAsync(UserProfile userProfile);
    }
}
