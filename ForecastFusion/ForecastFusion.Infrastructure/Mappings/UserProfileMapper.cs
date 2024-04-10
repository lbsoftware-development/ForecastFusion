using ForecastFusion.Infrastructure.Entities;
using DomainEntities = ForecastFusion.Domain.Entities;

namespace ForecastFusion.Infrastructure.Mappings
{
    public static class UserProfileMapper
    {
        public static UserProfile MapToInfra(DomainEntities.UserProfile profile)
        {
            return new UserProfile()
            {
                EmailAddress = profile.EmailAddress,
                Location = profile.Location,
                RowKey = profile.Id == Guid.Empty ? Guid.NewGuid().ToString() : profile.Id.ToString(),
                PartitionKey = profile.Country,
                Name = profile.Name
            };
        }

        public static DomainEntities.UserProfile MapToDomain(UserProfile userProfile)
        {
            return new DomainEntities.UserProfile()
            {
                EmailAddress = userProfile.EmailAddress,
                Country = userProfile.PartitionKey!,
                Location = userProfile.Location,
                Name = userProfile.Name,
                Id = Guid.Parse(userProfile.RowKey!)
            };
        }
    }
}
