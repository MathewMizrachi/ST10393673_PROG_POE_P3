using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
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

        private readonly ILogger<BlobStorageService> _logger;

        // Property to get the account name
        public string AccountName { get; private set; }

        public BlobStorageService(string connectionString, ILogger<BlobStorageService> logger)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString), "The connection string cannot be null or empty.");
            }

            _blobServiceClient = new BlobServiceClient(connectionString);
            _logger = logger;
            AccountName = ExtractAccountName(connectionString); // Extract the account name
            _logger.LogInformation($"BlobStorageService initialized for account: {AccountName}");
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
            try
            {
                // Validate the file size
                if (fileStream.Length > MaxFileSizeInBytes)
                {
                    _logger.LogWarning($"File size {fileStream.Length} exceeds the limit of {MaxFileSizeInBytes / (1024 * 1024)} MB.");
                    throw new InvalidOperationException($"File size exceeds the limit of {MaxFileSizeInBytes / (1024 * 1024)} MB.");
                }

                // Validate the file type
                string fileExtension = Path.GetExtension(blobName).ToLowerInvariant();
                if (!AllowedFileTypes.Contains(fileExtension))
                {
                    _logger.LogWarning($"Invalid file type: {fileExtension}. Allowed types are: {string.Join(", ", AllowedFileTypes)}.");
                    throw new InvalidOperationException("Invalid file type. Allowed types are: " + string.Join(", ", AllowedFileTypes));
                }

                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync(); // Create container if it doesn't exist

                var blobClient = containerClient.GetBlobClient(blobName);
                await blobClient.UploadAsync(fileStream, overwrite: true);
                _logger.LogInformation($"Blob {blobName} uploaded successfully to container {containerName}.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading blob {blobName} to container {containerName}: {ex.Message}");
                throw;
            }
        }

        public async Task<Stream> DownloadBlobAsync(string containerName, string blobName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(blobName);

                BlobDownloadInfo blobDownloadInfo = await blobClient.DownloadAsync();
                MemoryStream memoryStream = new MemoryStream();
                await blobDownloadInfo.Content.CopyToAsync(memoryStream);
                memoryStream.Position = 0; // Reset stream position
                _logger.LogInformation($"Blob {blobName} downloaded successfully from container {containerName}.");
                return memoryStream;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error downloading blob {blobName} from container {containerName}: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteBlobAsync(string containerName, string blobName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(blobName);
                await blobClient.DeleteIfExistsAsync(); // Deletes the blob if it exists
                _logger.LogInformation($"Blob {blobName} deleted successfully from container {containerName}.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting blob {blobName} from container {containerName}: {ex.Message}");
                throw;
            }
        }
    }
}
