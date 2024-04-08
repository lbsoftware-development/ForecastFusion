using Azure.Data.Tables;

namespace ForecastFusion.Application.Contracts
{
    public interface IAzureTableStorageService
    {
        Task<Result> UpsertEntityAsync<T>(T entity, string tableName) where T : ITableEntity;

        Task<Result<ITableEntity>> RetrieveEntityAsync<T>(string tableName, string partitionKey, string rowKey) where T : class, ITableEntity;
    }
}
