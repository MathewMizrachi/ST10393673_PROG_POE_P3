using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ST10393673_PROG6212_POE.Services
{
    public class BlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private const long MaxFileSizeInBytes = 5 * 1024 * 1024; // 5 MB limit
        private static readonly string[] AllowedFileTypes = { ".pdf", ".docx", ".xlsx" };

        // Property to get the account name
        public string AccountName { get; private set; }

        public BlobStorageService(string connectionString)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
            AccountName = ExtractAccountName(connectionString); // Extract the account name
        }

        private string ExtractAccountName(string connectionString)
        {
            // Split the connection string by semicolons and find the AccountName
            var parameters = connectionString.Split(';');
            var accountNameParameter = parameters.FirstOrDefault(p => p.StartsWith("AccountName=", StringComparison.OrdinalIgnoreCase));
            if (accountNameParameter != null)
            {
                // Return the value of AccountName parameter
                return accountNameParameter.Split('=')[1].Trim();
            }
            throw new InvalidOperationException("AccountName not found in the connection string.");
        }

        public async Task UploadBlobAsync(string containerName, string blobName, Stream fileStream)
        {
            // Validate the file size
            if (fileStream.Length > MaxFileSizeInBytes)
            {
                throw new InvalidOperationException($"File size exceeds the limit of {MaxFileSizeInBytes / (1024 * 1024)} MB.");
            }

            // Validate the file type
            string fileExtension = Path.GetExtension(blobName).ToLowerInvariant();
            if (!AllowedFileTypes.Contains(fileExtension))
            {
                throw new InvalidOperationException("Invalid file type. Allowed types are: " + string.Join(", ", AllowedFileTypes));
            }

            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(); // Create container if it doesn't exist

            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(fileStream, overwrite: true);
        }

        public async Task<Stream> DownloadBlobAsync(string containerName, string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            BlobDownloadInfo blobDownloadInfo = await blobClient.DownloadAsync();
            MemoryStream memoryStream = new MemoryStream();
            await blobDownloadInfo.Content.CopyToAsync(memoryStream);
            memoryStream.Position = 0; // Reset stream position
            return memoryStream;
        }

        public async Task DeleteBlobAsync(string containerName, string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync(); // Deletes the blob if it exists
        }
    }
}
