using Azure;
using Azure.Data.Tables;
using System;

namespace ST10393673_PROG6212_POE.Models
{
    // Implements ITableEntity to represent a user entity for Azure Table Storage
    public class UserEntity : ITableEntity
    {
        // Explicitly implement ITableEntity properties
        public string PartitionKey { get; set; } // Groups the users together
        public string RowKey { get; set; } // Unique identifier, email in this case

        public DateTimeOffset? Timestamp { get; set; } // Auto-generated timestamp by Azure Table Storage
        public ETag ETag { get; set; } // Auto-generated ETag for optimistic concurrency control

        // Custom properties of the user entity
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string JobTitle { get; set; }

        // For security reasons, store the hashed password
        public string PasswordHash { get; set; }

        // Store the email separately for easy retrieval
        public string Email { get; set; }

        // Constructor to initialize the PartitionKey and RowKey
        public UserEntity(string email)
        {
            // Initialize explicitly
            PartitionKey = "Users"; // This groups the users together
            RowKey = email; // Using email as the unique identifier (RowKey)
        }

        // Default constructor for TableEntity (needed for Azure Table Storage operations)
        public UserEntity() { }
    }
}
