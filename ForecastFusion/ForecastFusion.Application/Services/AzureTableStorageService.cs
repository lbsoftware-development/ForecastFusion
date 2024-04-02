using Azure;
using Azure.Data.Tables;
using ForecastFusion.Application.Contracts;
using System.Net;

namespace ForecastFusion.Application.Services
{
    public class AzureTableStorageService: IAzureTableStorageService
    {
        private TableServiceClient _tableServiceClient;
        public AzureTableStorageService(string connectionString)
        {
            _tableServiceClient = new TableServiceClient(connectionString);
        }

        public async Task UpsertEntityAsync<T>(T entity, string tableName) where T: ITableEntity 
        {
            ArgumentException.ThrowIfNullOrEmpty(tableName, nameof(tableName));
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));

            await _tableServiceClient.GetTableClient(tableName).UpdateEntityAsync(entity, Azure.ETag.All);
        }

        public async Task<ITableEntity> RetrieveEntityAsync<T>(string tableName, string partitionKey, string rowKey) where T : class, ITableEntity
        {
            Response<T> response = await _tableServiceClient.GetTableClient(tableName).GetEntityAsync<T>(partitionKey, rowKey, cancellationToken: CancellationToken.None);

            if (response.GetRawResponse().Status == (int)HttpStatusCode.OK)
            {
                return response.Value;
            }
            else
            {
                return default;
            }
        }
    }
}
