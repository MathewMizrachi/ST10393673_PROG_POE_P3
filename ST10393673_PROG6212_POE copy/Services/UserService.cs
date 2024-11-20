using Microsoft.Azure.Cosmos.Table;
using ST10393673_PROG6212_POE.Models;
using System.Threading.Tasks;

namespace ST10393673_PROG6212_POE.Services
{
    public class UserService
    {
        private readonly TableService _tableService;
        private const string TableName = "Users";

        public UserService(TableService tableService)
        {
            _tableService = tableService;
        }

        // Method to register a new user
        public async Task<bool> RegisterUser(UserEntity user)
        {
            // Get or create the Users table
            var table = await _tableService.GetOrCreateTableAsync(TableName);
            // Insert or merge the user entity into the table
            return await _tableService.InsertOrMergeEntityAsync(table, user);
        }

        // Method to retrieve a user by partition key and row key
        public async Task<UserEntity> GetUser(string partitionKey, string rowKey)
        {
            // Get or create the Users table
            var table = await _tableService.GetOrCreateTableAsync(TableName);
            // Retrieve the user entity from the table
            return await _tableService.RetrieveEntityAsync<UserEntity>(table, partitionKey, rowKey);
        }

        // Method to update user details
        public async Task<bool> UpdateUser(UserEntity user)
        {
            // Get or create the Users table
            var table = await _tableService.GetOrCreateTableAsync(TableName);
            // Insert or merge the user entity into the table (this serves as an update if the entity exists)
            return await _tableService.InsertOrMergeEntityAsync(table, user);
        }

        // Method to delete a user
        public async Task<bool> DeleteUser(string partitionKey, string rowKey)
        {
            // Get or create the Users table
            var table = await _tableService.GetOrCreateTableAsync(TableName);
            // Retrieve the user entity from the table
            var user = await _tableService.RetrieveEntityAsync<UserEntity>(table, partitionKey, rowKey);

            // If the user exists, delete it
            if (user != null)
            {
                return await _tableService.DeleteEntityAsync(table, user);
            }

            // Return false if the user was not found
            return false;
        }
    }
}
