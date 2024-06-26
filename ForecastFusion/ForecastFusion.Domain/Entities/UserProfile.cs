﻿namespace ForecastFusion.Domain.Entities
{
    public class UserProfile
    {
        public Guid Id { get; set; }

        public required string Country { get; set; }

        public required string Name { get; set; }

        public required string Location { get; set; }

        public string? EmailAddress { get; set; }

    }
}
