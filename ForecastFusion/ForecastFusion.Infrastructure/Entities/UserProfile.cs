using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForecastFusion.Infrastructure.Entities
{
    public class UserProfile: AzureTableEntity
    {     
        public required string Name { get; set; }

        public required string Country { get; set; }

        public required string Location { get; set; }

        public string? EmailAddress { get; set; }
    }
}
