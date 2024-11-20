using Microsoft.Azure.Cosmos.Table;
using System;

namespace ST10393673_PROG6212_POE.Models
{
    public class UserEntity : TableEntity
    {
        public UserEntity(string email)
        {
            PartitionKey = "Users";
            RowKey = email;
        }

        public UserEntity() { }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string JobTitle { get; set; }

        public string Email
        {
            get { return RowKey; }
            set { RowKey = value; }
        }

        public string PasswordHash { get; set; }
        public DateTime RegisteredDate { get; set; }

        public string Role { get; set; } = "User";
        public bool IsActive { get; set; } = true;

        // New fields
        public DateTime LastLoginDate { get; set; }
        public bool IsLockedOut { get; set; }
    }

}
