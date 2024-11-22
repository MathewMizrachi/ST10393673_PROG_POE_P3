using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Azure.Data.Tables;
using System;
using System.Threading.Tasks;
using ST10393673_PROG6212_POE.Models;

namespace ST10393673_PROG6212_POE.Services
{
    public class StorageTestService
    {
        private readonly string _connectionString;

        public StorageTestService(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Test Azure Table Storage Connection by attempting to retrieve an entity
        public async Task<bool> TestAzureTableStorageConnectionAsync()
        {
            try
            {
                var tableServiceClient = new TableServiceClient(_connectionString);
                var tableClient = tableServiceClient.GetTableClient("UserTable"); // Use your actual table name
                await tableClient.GetEntityAsync<UserEntity>("Users", "testuser"); // Try fetching a sample user
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to Azure Table Storage: {ex.Message}");
                return false;
            }
        }

        // Test Azure Blob Storage Connection by attempting to list containers
        public async Task<bool> TestAzureBlobStorageConnectionAsync()
        {
            try
            {
                var blobServiceClient = new BlobServiceClient(_connectionString);
                await foreach (var container in blobServiceClient.GetBlobContainersAsync())
                {
                    Console.WriteLine($"Found container: {container.Name}");
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to Azure Blob Storage: {ex.Message}");
                return false;
            }
        }

        // Test Azure Queue Storage Connection by attempting to list queues
        public async Task<bool> TestAzureQueueStorageConnectionAsync()
        {
            try
            {
                var queueServiceClient = new QueueServiceClient(_connectionString);
                await foreach (var queue in queueServiceClient.GetQueuesAsync())
                {
                    Console.WriteLine($"Found queue: {queue.Name}");
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to Azure Queue Storage: {ex.Message}");
                return false;
            }
        }
    }
}
