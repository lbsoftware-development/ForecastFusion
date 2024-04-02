using Azure;
using Azure.Data.Tables;

namespace ForecastFusion.Infrastructure.Entities
{
    internal abstract class AzureTableEntity : ITableEntity
    {
        public string? PartitionKey { get; set ; }
        public string? RowKey { get; set ; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
