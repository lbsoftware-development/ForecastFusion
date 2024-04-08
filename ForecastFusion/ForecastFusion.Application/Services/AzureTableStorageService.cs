using Azure;
using Azure.Data.Tables;
using ForecastFusion.Application.Contracts;
using System.Net;

namespace ForecastFusion.Application.Services
{
    public class AzureTableStorageService : IAzureTableStorageService
    {
        private TableServiceClient _tableServiceClient;
        public AzureTableStorageService(string connectionString)
        {
            _tableServiceClient = new TableServiceClient(connectionString);
        }

        public async Task<Result> UpsertEntityAsync<T>(T entity, string tableName) where T : ITableEntity
        {
            ArgumentException.ThrowIfNullOrEmpty(tableName, nameof(tableName));
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));

            try
            {
                var updateResult = await _tableServiceClient.GetTableClient(tableName).UpsertEntityAsync(entity, TableUpdateMode.Replace);
                return Result.Success();
            }
            catch (RequestFailedException ex)
            {
                var returnResult = Result.Failure(ex);
                returnResult.HttpStatusCode = (HttpStatusCode)ex.Status;
                return returnResult;
            }

        }

        public async Task<Result<ITableEntity>> RetrieveEntityAsync<T>(string tableName, string partitionKey, string rowKey) where T : class, ITableEntity
        {
            try
            {
                Response<T> response = await _tableServiceClient.GetTableClient(tableName).GetEntityAsync<T>(partitionKey, rowKey, cancellationToken: CancellationToken.None);

                return Result<ITableEntity>.Success(response.Value);

            }
            catch (RequestFailedException ex)
            {
                var returnResult = Result<ITableEntity>.Failure(ex);
                returnResult.HttpStatusCode = (HttpStatusCode)ex.Status;
                return returnResult;
            }

        }
    }
}
