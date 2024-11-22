using Azure.Data.Tables;
using System;
using System.Threading.Tasks;

namespace ST10393673_PROG6212_POE.Services
{
    public class UserService
    {
        private readonly string _connectionString;
        private readonly TableServiceClient _tableServiceClient;

        public UserService(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _tableServiceClient = new TableServiceClient(_connectionString);
        }

        // Retrieve or create a table
        public async Task<TableClient> GetOrCreateTableAsync(string tableName)
        {
            var tableClient = _tableServiceClient.GetTableClient(tableName);
            await tableClient.CreateIfNotExistsAsync();  // Ensure the table exists
            return tableClient;
        }

        // Insert or merge an entity in the table
        public async Task<bool> InsertOrMergeEntityAsync<T>(TableClient tableClient, T entity) where T : class, ITableEntity
        {
            try
            {
                await tableClient.UpsertEntityAsync(entity, TableUpdateMode.Merge);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting or merging entity: {ex.Message}");
                return false;
            }
        }

        // Retrieve an entity from the table
        public async Task<T> RetrieveEntityAsync<T>(TableClient tableClient, string partitionKey, string rowKey) where T : class, ITableEntity
        {
            try
            {
                var entity = await tableClient.GetEntityAsync<T>(partitionKey, rowKey);
                return entity.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving entity: {ex.Message}");
                return null;
            }
        }

        // Delete an entity from the table
        public async Task<bool> DeleteEntityAsync<T>(TableClient tableClient, T entity) where T : class, ITableEntity
        {
            try
            {
                await tableClient.DeleteEntityAsync(entity.PartitionKey, entity.RowKey);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting entity: {ex.Message}");
                return false;
            }
        }
    }
}
