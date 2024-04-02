using Azure.Data.Tables;

namespace ForecastFusion.Application.Contracts
{
    public interface IAzureTableStorageService
    {
        Task UpsertEntityAsync<T>(T entity, string tableName) where T : ITableEntity;

        Task<ITableEntity> RetrieveEntityAsync<T>(string tableName, string partitionKey, string rowKey) where T : class, ITableEntity;
    }
}
