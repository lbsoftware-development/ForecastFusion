using Azure;
using Azure.Data.Tables;

namespace ForecastFusion.Infrastructure.Entities
{
    public abstract class AzureTableEntity : ITableEntity
    {
        public string? PartitionKey { get; set ; }
        public string? RowKey { get; set ; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
