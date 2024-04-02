using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForecastFusion.Domain.Entities
{
    public class UserProfile
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public required string Location { get; set; }

        public string? EmailAddress { get; set; }

    }
}
