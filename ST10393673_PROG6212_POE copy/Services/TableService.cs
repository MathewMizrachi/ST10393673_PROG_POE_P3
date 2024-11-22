using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ST10393673_PROG6212_POE.Services
{
    public class TableService
    {
        private readonly string _connectionString;
        private readonly TableServiceClient _tableServiceClient;
        private readonly ILogger<TableService> _logger;

        // Constructor with ILogger for better error logging
        public TableService(string connectionString, ILogger<TableService> logger)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _tableServiceClient = new TableServiceClient(_connectionString);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Retrieve or create a table
        public async Task<TableClient> GetOrCreateTableAsync(string tableName, CancellationToken cancellationToken = default)
        {
            try
            {
                var tableClient = _tableServiceClient.GetTableClient(tableName);
                await tableClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
                return tableClient;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating or retrieving table: {tableName}");
                return null;
            }
        }

        // Insert or merge an entity in the table
        public async Task<bool> InsertOrMergeEntityAsync<T>(TableClient tableClient, T entity, CancellationToken cancellationToken = default) where T : class, ITableEntity
        {
            try
            {
                // Upsert the entity, merging the existing one if it exists
                await tableClient.UpsertEntityAsync(entity, TableUpdateMode.Merge, cancellationToken: cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting or merging entity.");
                return false;
            }
        }

        // Retrieve an entity from the table
        public async Task<T> RetrieveEntityAsync<T>(TableClient tableClient, string partitionKey, string rowKey, CancellationToken cancellationToken = default) where T : class, ITableEntity
        {
            try
            {
                // Retrieve the entity from the table by partition and row key
                var entity = await tableClient.GetEntityAsync<T>(partitionKey, rowKey, cancellationToken: cancellationToken);
                return entity.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving entity: PartitionKey={partitionKey}, RowKey={rowKey}");
                return null;
            }
        }

        // Delete an entity from the table
        public async Task<bool> DeleteEntityAsync<T>(TableClient tableClient, T entity, CancellationToken cancellationToken = default) where T : class, ITableEntity
        {
            try
            {
                // Delete the entity from the table by partition and row key
                await tableClient.DeleteEntityAsync(entity.PartitionKey, entity.RowKey, cancellationToken: cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting entity.");
                return false;
            }
        }
    }
}
