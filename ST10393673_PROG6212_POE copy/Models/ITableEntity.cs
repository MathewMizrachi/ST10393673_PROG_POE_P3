using Azure;
using Microsoft.Azure.Cosmos.Table;

namespace ST10393673_PROG6212_POE.Models
{
    public interface ITableEntity
    {
        string PartitionKey { get; set; }
        string RowKey { get; set; }
        DateTimeOffset? Timestamp { get; set; }
        ETag ETag { get; set; }
    }
}
