using HillMetrics.MIND.API.SDK.V1;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Refit;
using System.Security.Cryptography;
using HillMetrics.Normalized.Domain.Contracts.Repository;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset;

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
                var fileUpload = response.Data;

                if (fileUpload == null)
                {
                    _logger.LogError("File upload not found for ID: {FileUploadId}", fileUploadId);
                    return null;
                }

                Stream fileStream;

                if (fileUpload.FluxFetchingContentId.HasValue)
                {
                    // Si le fichier vient d'un FluxFetching
                    _logger.LogInformation("Fetching file from FluxFetching with ID: {FluxFetchingContentId}",
                        fileUpload.FluxFetchingContentId.Value);

                    var fpcResponse = await _mindApi.GetFetchingContentAsync(fileUpload.FluxFetchingContentId.Value);

                    fileStream = await _mindApi.GetFile(fpcResponse.Data.RawId!);
                }
                else if (fileUpload.FileMetadataId.HasValue)
                {
                    // Si le fichier vient d'un FileMetadata
                    _logger.LogInformation("Fetching file from FileMetadata with ID: {FileMetadataId}",
                        fileUpload.FileMetadataId.Value);
                    fileStream = await _mindApi.DownloadStoredFile(fileUpload.FileMetadataId.Value);
                }
                else
                {
                    _logger.LogError("File has no source (neither FluxFetching nor FileMetadata) for ID: {FileUploadId}",
                        fileUploadId);
                    throw new InvalidOperationException("File has no source (neither FluxFetching nor FileMetadata)");
                }

                return fileStream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting file stream for {FileId}", fileUploadId);
                return null;
            }
        }

        /// <summary>
        /// Gets the size of a file without downloading the full content.
        /// </summary>
        /// <param name="fileUploadId">The ID of the file to check.</param>
        /// <returns>The file size in bytes, or null if not found or error.</returns>
        public async Task<long?> GetFileSizeAsync(int fileUploadId)
        {
            try
            {
                var response = await _mindApi.GetFileUploadAsync(fileUploadId);
                var fileUpload = response.Data;

                if (fileUpload == null)
                {
                    _logger.LogError("File upload not found for ID: {FileUploadId}", fileUploadId);
                    return null;
                }

                if (fileUpload.FluxFetchingContentId.HasValue)
                {
                    // Si le fichier vient d'un FluxFetching, utiliser l'API GetFileSize
                    var fpcResponse = await _mindApi.GetFetchingContentAsync(fileUpload.FluxFetchingContentId.Value);

                    if (!string.IsNullOrEmpty(fpcResponse.Data.RawId))
                    {
                        try
                        {
                            var sizeResponse = await _mindApi.GetFileSize(fpcResponse.Data.RawId);
                            var sizeInfo = System.Text.Json.JsonSerializer.Deserialize<dynamic>(sizeResponse.ToString());
                            return sizeInfo.GetProperty("sizeInBytes").GetInt64();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Could not get file size for raw ID: {RawId}", fpcResponse.Data.RawId);
                            return null;
                        }
                    }
                }
                else if (fileUpload.FileMetadataId.HasValue)
                {
                    // Pour FileMetadata, on pourrait utiliser FileSize si disponible sur l'objet FileUpload
                    // Ou implémenter une méthode GetStoredFileSize dans l'API
                    _logger.LogInformation("Getting size for FileMetadata with ID: {FileMetadataId}", fileUpload.FileMetadataId.Value);
                    // Pour l'instant, retourner null pour forcer le téléchargement si nécessaire
                    return fileUpload.FileSize > 0 ? fileUpload.FileSize : null;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting file size for {FileId}", fileUploadId);
                return null;
            }
        }
    }
}