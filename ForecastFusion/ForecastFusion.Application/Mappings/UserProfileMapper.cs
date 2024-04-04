using ForecastFusion.Application.DTOs;
using ForecastFusion.Domain.Entities;

namespace ForecastFusion.Application.Mappings
{
    public static class UserProfileMapper
    {
        public static UserProfileDto MapToDto(UserProfile userProfile)
        {
            return new UserProfileDto
            {
                Location = userProfile.Location,
                Name = userProfile.Name,
                EmailAddress = userProfile.EmailAddress,
                Id = userProfile.Id,
                Country = userProfile.Country
            };
        }

        public static UserProfile MapToDomain(UserProfileDto userProfileDto)
        {
            return new UserProfile
            {
                Location = userProfileDto.Location,
                Name = userProfileDto.Name,
                EmailAddress = userProfileDto.EmailAddress,
                Id = userProfileDto.Id,
                Country = userProfileDto.Country
            };
        }
    }
}
