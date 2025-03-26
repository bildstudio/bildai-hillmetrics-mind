using HillMetrics.MIND.API.SDK.V1;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Refit;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset;
using System.Security.Cryptography;

namespace HillMetrics.MIND.FrontApp.Services
{
    /// <summary>
    /// Service for handling file upload operations.
    /// </summary>
    public class FileUploadService
    {
        private readonly IMindAPI _mindApi;
        private readonly ILogger<FileUploadService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileUploadService"/> class.
        /// </summary>
        /// <param name="mindApi">The MIND API client.</param>
        /// <param name="logger">The logger.</param>
        public FileUploadService(
            IMindAPI mindApi,
            ILogger<FileUploadService> logger)
        {
            _mindApi = mindApi;
            _logger = logger;
        }

        /// <summary>
        /// Calculates the SHA256 hash of a file's content.
        /// </summary>
        /// <param name="fileStream">The file stream to hash.</param>
        /// <returns>The hash as a hexadecimal string.</returns>
        private async Task<string> CalculateFileHashAsync(Stream fileStream)
        {
            using var sha256 = SHA256.Create();
            fileStream.Position = 0;
            var hashBytes = await sha256.ComputeHashAsync(fileStream);
            fileStream.Position = 0; // Reset position for further use

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// Uploads a file and saves its information to the database.
        /// </summary>
        /// <param name="file">The file to upload.</param>
        /// <returns>A tuple containing the created FileUpload and a boolean indicating if it's a duplicate.</returns>
        public async Task<(FileUpload FileUpload, bool IsDuplicate)> UploadFileAsync(IBrowserFile file)
        {
            try
            {
                const int maxFileSize = 10485760; // 10MB
                using var browserStream = file.OpenReadStream(maxAllowedSize: maxFileSize);
                using var memoryStream = new MemoryStream();

                await browserStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                // Créer un StreamPart pour l'upload
                var streamContent = new StreamPart(memoryStream, file.Name, file.ContentType);

                // Upload via l'API
                var response = await _mindApi.CreateFileUploadAsync(streamContent);

                return (response.Data, false); // IsDuplicate sera géré par l'API
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file {FileName}", file.Name);
                throw;
            }
        }

        /// <summary>
        /// Gets all uploaded files.
        /// </summary>
        /// <returns>A list of file uploads.</returns>
        public async Task<List<FileUpload>> GetAllFilesAsync()
        {
            try
            {
                var response = await _mindApi.GetAllFileUploadsAsync();
                return response.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all files");
                throw;
            }
        }

        /// <summary>
        /// Gets a file by its ID.
        /// </summary>
        /// <param name="fileId">The ID of the file to get.</param>
        /// <returns>The file upload, or null if not found.</returns>
        public async Task<FileUpload?> GetFileByIdAsync(int fileId)
        {
            try
            {
                var response = await _mindApi.GetFileUploadAsync(fileId);
                return response.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting file {FileId}", fileId);
                throw;
            }
        }

        /// <summary>
        /// Deletes a file by its ID.
        /// </summary>
        /// <param name="fileId">The ID of the file to delete.</param>
        /// <returns>True if the file was deleted, false otherwise.</returns>
        public async Task<bool> DeleteFileAsync(int fileId)
        {
            try
            {
                var response = await _mindApi.DeleteFileUploadAsync(fileId);
                return response.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file {FileId}", fileId);
                return false;
            }
        }

        /// <summary>
        /// Uploads a file from a memory stream.
        /// </summary>
        /// <param name="fileStream">The memory stream containing the file data.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="contentType">The content type of the file.</param>
        /// <param name="fileSize">The size of the file in bytes.</param>
        /// <returns>A tuple containing the created FileUpload and a boolean indicating if it's a duplicate.</returns>
        public async Task<(FileUpload FileUpload, bool IsDuplicate)> UploadFileFromStreamAsync(
            MemoryStream fileStream,
            string fileName,
            string contentType,
            long fileSize)
        {
            try
            {
                if (fileSize > 10485760) // 10MB
                {
                    throw new ArgumentException("File size exceeds maximum allowed size of 10MB");
                }

                fileStream.Position = 0;
                var streamContent = new StreamPart(fileStream, fileName, contentType);

                var response = await _mindApi.CreateFileUploadAsync(streamContent);
                return (response.Data, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file {FileName}", fileName);
                throw;
            }
        }

        /// <summary>
        /// Gets a file stream for a specific file ID.
        /// </summary>
        /// <param name="fileUploadId">The ID of the file to get.</param>
        /// <returns>The file stream, or null if not found.</returns>
        public async Task<Stream?> GetFileStreamAsync(int fileUploadId)
        {
            try
            {
                var response = await _mindApi.GetFileUploadAsync(fileUploadId);
                var fileStream = await _mindApi.GetFile(response.Data.FileMetadata.SourceId);
                return fileStream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting file stream for {FileId}", fileUploadId);
                return null;
            }
        }
    }
}