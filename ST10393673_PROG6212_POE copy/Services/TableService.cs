using Microsoft.Azure.Cosmos.Table;
using System;
using System.Threading.Tasks;

namespace ST10393673_PROG6212_POE.Services
{
    public class TableService
    {
        private readonly CloudTableClient _tableClient;

        public TableService(string storageConnectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            _tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
        }

        // Create a table if it doesn't already exist
        public async Task<CloudTable> GetOrCreateTableAsync(string tableName)
        {
            var table = _tableClient.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }

        // Insert or update an entity in the table
        public async Task<bool> InsertOrMergeEntityAsync<T>(CloudTable table, T entity) where T : TableEntity
        {
            try
            {
                var insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
                var result = await table.ExecuteAsync(insertOrMergeOperation);
                return result.HttpStatusCode == 204;
            }
            catch (StorageException e)
            {
                Console.WriteLine($"Error during InsertOrMerge operation: {e.Message}");
                return false;
            }
        }

        // Retrieve an entity from the table
        public async Task<T> RetrieveEntityAsync<T>(CloudTable table, string partitionKey, string rowKey) where T : TableEntity
        {
            var retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            var result = await table.ExecuteAsync(retrieveOperation);
            return result.Result as T;
        }

        // Delete an entity from the table
        public async Task<bool> DeleteEntityAsync<T>(CloudTable table, T entity) where T : TableEntity
        {
            try
            {
                var deleteOperation = TableOperation.Delete(entity);
                var result = await table.ExecuteAsync(deleteOperation);
                return result.HttpStatusCode == 204;
            }
            catch (StorageException e)
            {
                Console.WriteLine($"Error during Delete operation: {e.Message}");
                return false;
            }
        }
    }
}
