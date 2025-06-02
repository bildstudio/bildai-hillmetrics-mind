using HillMetrics.MIND.API.SDK.V1;
using System.Text.Json;
using System.Text;
using Microsoft.JSInterop;
using System.IO;
using System.IO.Compression;
using HillMetrics.MIND.API.Contracts.Requests.AiDataset;
using HillMetrics.MIND.API.Contracts.Responses.AiDataset;
using HillMetrics.Core.Search;
using System.Text.RegularExpressions;
using HillMetrics.Core.Financial;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset;

namespace HillMetrics.MIND.FrontApp.Services
{
    /// <summary>
    /// Service for exporting file data mappings to various formats.
    /// </summary>
    public class MappingExportService
    {
        private readonly IMindAPI _mindApi;
        private readonly IJSRuntime _jsRuntime;
        private readonly ILogger<MappingExportService> _logger;
        private readonly FileUploadService _fileUploadService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingExportService"/> class.
        /// </summary>
        /// <param name="mindApi">The MIND API client.</param>
        /// <param name="jsRuntime">The JavaScript runtime.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="fileUploadService">Service to handle file uploads/downloads.</param>
        public MappingExportService(
            IMindAPI mindApi,
            IJSRuntime jsRuntime,
            ILogger<MappingExportService> logger,
            FileUploadService fileUploadService)
        {
            _mindApi = mindApi;
            _jsRuntime = jsRuntime;
            _logger = logger;
            _fileUploadService = fileUploadService;
        }

        /// <summary>
        /// Exports a specific mapping to JSON format.
        /// </summary>
        /// <param name="mappingId">The ID of the mapping to export.</param>
        /// <returns>A JSON string representing the mapping.</returns>
        public async Task<string> ExportMappingToJsonAsync(int mappingId)
        {
            try
            {
                var response = await _mindApi.GetFileMappingAsync(mappingId);

                var mapping = response.Data;
                if (mapping == null)
                {
                    throw new ArgumentException($"Mapping with ID {mappingId} not found.");
                }

                var mappingData = new
                {
                    MappingId = mapping.Id,
                    File = new
                    {
                        FileId = mapping.FileUpload.Id,
                        FileName = mapping.FileUpload.FileName,
                        FileType = mapping.FileUpload.FileType,
                        ContentType = mapping.FileUpload.ContentType,
                        FileSize = mapping.FileUpload.FileSize,
                        UploadedAt = mapping.FileUpload.UploadedAt
                    },
                    DataPoint = new
                    {
                        DataPointId = mapping.FinancialDataPoint.Id,
                        Name = mapping.FinancialDataPoint.Name
                    },
                    Elements = mapping.ElementValues.Select(e => new
                    {
                        ElementValueId = e.Id,
                        PropertyName = e.FinancialDataPointElement.PropertyName,
                        Value = e.ExtractedValue,
                        DataType = e.PropertyDataType.Name.ToString(),
                        Description = e.FinancialDataPointElement.Description
                    }).ToList()
                };

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                return JsonSerializer.Serialize(mappingData, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting mapping {MappingId} to JSON", mappingId);
                throw;
            }
        }

        /// <summary>
        /// Exports all mappings for a specific file to JSON format.
        /// </summary>
        /// <param name="fileUploadId">The ID of the file to export mappings for.</param>
        /// <returns>A JSON string representing all mappings for the file.</returns>
        public async Task<string> ExportFileMappingsToJsonAsync(int fileUploadId)
        {
            try
            {
                // Get file details
                var fileResponse = await _mindApi.GetFileUploadAsync(fileUploadId);

                var fileUpload = fileResponse.Data;
                if (fileUpload == null)
                {
                    throw new Exception($"File with ID {fileUploadId} not found.");
                }

                // Get mappings for the file
                var mappingsResponse = await _mindApi.GetMappingsByFileUploadAsync(fileUploadId);

                var mappings = mappingsResponse.Data;
                if (!mappings.Any())
                {
                    throw new Exception($"No mappings found for file with ID {fileUploadId}.");
                }

                // Create the root JSON object
                var rootObject = new Dictionary<string, object>();


                // Group mappings by DataPoint.Name
                var mappingsByDataPoint = mappings
                    .GroupBy(m => m.FinancialDataPoint.Name)
                    .ToDictionary(g => g.Key, g => g.ToList());

                foreach (var group in mappingsByDataPoint)
                {
                    string dataPointName = group.Key;
                    var dataPointMappings = group.Value;
                    var identifierArray = new List<Dictionary<string, object>>();
                    foreach (var mapping in dataPointMappings)
                    {
                        var identifierObject = CreateElementsDictionary(mapping);
                        identifierArray.Add(identifierObject);
                    }
                    rootObject[dataPointName] = identifierArray;
                }

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                return JsonSerializer.Serialize(rootObject, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting file mappings to JSON");
                throw;
            }
        }

        private Dictionary<string, object> CreateElementsDictionary(FileDataMapping mapping)
        {
            var elementsDictionary = new Dictionary<string, object>();

            foreach (var element in mapping.FinancialDataPoint.Elements)
            {
                var elementValue = mapping.ElementValues
                    .FirstOrDefault(ev => ev.FinancialDataPointElementId == element.Id);

                if (elementValue != null && !string.IsNullOrEmpty(elementValue.ExtractedValue))
                {
                    elementsDictionary[element.PropertyName] = new
                    {
                        DataType = elementValue.PropertyDataType.Name.ToString(),
                        MappingType = element.MappingPrimitiveValue.ToString(),
                        Value = elementValue.ExtractedValue.Trim()
                    };
                }
                else
                {
                    elementsDictionary[element.PropertyName] = "";
                }
            }

            return elementsDictionary;
        }

        /// <summary>
        /// Downloads a JSON file to the client.
        /// </summary>
        /// <param name="jsonContent">The JSON content to download.</param>
        /// <param name="fileName">The name of the file to download.</param>
        public async Task DownloadJsonFileAsync(string jsonContent, string fileName)
        {
            try
            {
                if (!fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    fileName += ".json";
                }

                var fileBytes = Encoding.UTF8.GetBytes(jsonContent);
                var base64Content = Convert.ToBase64String(fileBytes);

                await _jsRuntime.InvokeVoidAsync(
                    "downloadFileFromBase64",
                    fileName,
                    base64Content,
                    "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading JSON file {FileName}", fileName);
                throw;
            }
        }

        /// <summary>
        /// Exports the provided file mappings into a ZIP archive.
        /// Each file gets its own folder containing the original file and its mapping JSON.
        /// </summary>
        /// <param name="fileUploadsToExport">The collection of file uploads to include in the export.</param>
        /// <returns>A tuple containing the ZIP file content as bytes and the filename. Returns null bytes if no files are provided or an error occurs.</returns>
        public async Task<(byte[]? ZipBytes, string FileName)> ExportFilteredMappingsToZipAsync(IEnumerable<FileUploadSearchResponse> fileUploadsToExport)
        {
            string tempDirectoryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            string zipFileName = $"Export_Mappings_{DateTime.UtcNow:yyyyMMddHHmmss}.zip";
            string zipFilePath = Path.Combine(Path.GetTempPath(), zipFileName);
            bool directoryCreated = false;
            int filesProcessedCount = 0;

            try
            {
                if (fileUploadsToExport == null || !fileUploadsToExport.Any())
                {
                    _logger.LogWarning("No file uploads provided for export.");
                    return (null, zipFileName);
                }

                _logger.LogInformation("Found {Count} files to export. Creating temporary directory: {TempPath}", fileUploadsToExport.Count(), tempDirectoryPath);
                Directory.CreateDirectory(tempDirectoryPath);
                directoryCreated = true;
                
                // Get all financial data points to include unmapped ones
                _logger.LogInformation("Fetching all financial data points to include unmapped ones in the export");
                var allDataPointsResponse = await _mindApi.GetAllFinancialDataPointsAsync();
                var allDataPoints = allDataPointsResponse.Data ?? new List<FinancialDataPoint>();
                _logger.LogInformation("Retrieved {Count} financial data points", allDataPoints.Count);

                foreach (var fileUpload in fileUploadsToExport)
                {
                    try
                    {
                        string sanitizedFolderName = SanitizeFileName(Path.GetFileNameWithoutExtension(fileUpload.FileName));
                        string fileSpecificTempPath = Path.Combine(tempDirectoryPath, sanitizedFolderName);
                        Directory.CreateDirectory(fileSpecificTempPath);

                        _logger.LogInformation("Processing file ID {FileId}: {FileName}", fileUpload.Id, fileUpload.FileName);

                        // 1. Download Original File
                        try
                        {
                            _logger.LogDebug("Downloading original file for ID {FileId}", fileUpload.Id);
                            using var fileStream = await _fileUploadService.GetFileStreamAsync(fileUpload.Id);
                            if (fileStream != null)
                            {
                                // Copy to MemoryStream first to ensure Length property is available and stream is seekable
                                using var memoryStream = new MemoryStream();
                                await fileStream.CopyToAsync(memoryStream);

                                if (memoryStream.Length > 0) // Check length on MemoryStream
                                {
                                    memoryStream.Position = 0; // Reset MemoryStream position before reading
                                    string originalFilePath = Path.Combine(fileSpecificTempPath, fileUpload.FileName);
                                    using var fileWriteStream = new FileStream(originalFilePath, FileMode.Create, FileAccess.Write);
                                    await memoryStream.CopyToAsync(fileWriteStream); // Copy from MemoryStream to FileStream
                                    _logger.LogDebug("Successfully saved original file to {Path}", originalFilePath);
                                }
                                else
                                {
                                     _logger.LogWarning("Retrieved file stream was empty for file ID {FileId}", fileUpload.Id);
                                }
                            }
                            else
                            {
                                _logger.LogWarning("Could not retrieve file stream for file ID {FileId}", fileUpload.Id);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error downloading original file for ID {FileId}", fileUpload.Id);
                            // Decide if you want to continue without the original file or skip this entry
                        }

                        // 2. Generate Mapping JSON with all data points, including unmapped ones
                        try
                        {
                            _logger.LogDebug("Generating enhanced mapping JSON for file ID {FileId}", fileUpload.Id);
                            string jsonContent = await ExportFileMappingsWithAllDataPointsAsync(fileUpload.Id, allDataPoints);
                            if (!string.IsNullOrWhiteSpace(jsonContent))
                            {
                                string jsonFileName = $"{sanitizedFolderName}_mapping.json";
                                string jsonFilePath = Path.Combine(fileSpecificTempPath, jsonFileName);
                                await File.WriteAllTextAsync(jsonFilePath, jsonContent, Encoding.UTF8);
                                _logger.LogDebug("Successfully saved enhanced mapping JSON to {Path}", jsonFilePath);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Could not generate or save mapping JSON for file ID {FileId}.", fileUpload.Id);
                            // Continue processing other files
                        }
                        filesProcessedCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process file entry for export: ID {FileId}, Name {FileName}", fileUpload.Id, fileUpload.FileName);
                        // Continue to next file
                    }
                }

                if (filesProcessedCount == 0)
                {
                     _logger.LogWarning("Although files were found, none could be successfully processed for the export.");
                     return (null, zipFileName);
                }

                _logger.LogInformation("Creating ZIP archive at {ZipPath} from directory {TempPath}", zipFilePath, tempDirectoryPath);
                ZipFile.CreateFromDirectory(tempDirectoryPath, zipFilePath);

                _logger.LogDebug("Reading ZIP file into byte array.");
                byte[] zipBytes = await File.ReadAllBytesAsync(zipFilePath);

                _logger.LogInformation("Successfully created ZIP file with {Count} processed entries.", filesProcessedCount);
                return (zipBytes, zipFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the filtered mapping export process.");
                return (null, zipFileName); // Return null bytes on error
            }
            finally
            {
                // Clean up temporary files and directory
                try
                {
                    if (File.Exists(zipFilePath))
                    {
                        _logger.LogDebug("Deleting temporary ZIP file: {ZipPath}", zipFilePath);
                        File.Delete(zipFilePath);
                    }
                    if (directoryCreated && Directory.Exists(tempDirectoryPath))
                    {
                        _logger.LogDebug("Deleting temporary directory: {TempPath}", tempDirectoryPath);
                        Directory.Delete(tempDirectoryPath, true);
                    }
                }
                catch (IOException ioEx)
                {
                    // Log cleanup errors but don't throw, as the main operation might have succeeded
                    _logger.LogError(ioEx, "Error cleaning up temporary files/directory during export.");
                }
            }
        }

        /// <summary>
        /// Exports file mappings to JSON format, including all data points even if they're not mapped.
        /// </summary>
        /// <param name="fileUploadId">The ID of the file to export mappings for.</param>
        /// <param name="allDataPoints">All available financial data points.</param>
        /// <returns>A JSON string representing all mappings for the file with unmapped data points included.</returns>
        private async Task<string> ExportFileMappingsWithAllDataPointsAsync(int fileUploadId, List<FinancialDataPoint> allDataPoints)
        {
            try
            {
                // Get file details
                var fileResponse = await _mindApi.GetFileUploadAsync(fileUploadId);

                var fileUpload = fileResponse.Data;
                if (fileUpload == null)
                {
                    throw new Exception($"File with ID {fileUploadId} not found.");
                }

                // Get mappings for the file
                var mappingsResponse = await _mindApi.GetMappingsByFileUploadAsync(fileUploadId);
                var mappings = mappingsResponse.Data ?? new List<FileDataMapping>();
                
                _logger.LogDebug("Found {Count} mappings for file ID {FileId}", mappings.Count, fileUploadId);

                // Create the root JSON object
                var rootObject = new Dictionary<string, object>();

                // Create a dictionary of mapped data points for quick lookup
                var mappedDataPoints = mappings
                    .GroupBy(m => m.FinancialDataPoint.Name)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // Process all data points, including those that aren't mapped
                foreach (var dataPoint in allDataPoints)
                {
                    string dataPointName = dataPoint.Name.Trim();
                    
                    if (mappedDataPoints.TryGetValue(dataPointName, out var dataPointMappings))
                    {
                        // This data point is mapped - use existing logic
                        var identifierArray = new List<Dictionary<string, object>>();
                        foreach (var mapping in dataPointMappings)
                        {
                            var identifierObject = CreateElementsDictionary(mapping);
                            identifierArray.Add(identifierObject);
                        }
                        rootObject[dataPointName] = identifierArray;
                    }
                    else
                    {
                        // This data point is not mapped - create empty entry
                        var emptyMapping = new List<Dictionary<string, object>>();
                        var emptyObject = new Dictionary<string, object>();
                        
                        // Add empty entries for each element in the data point
                        foreach (var element in dataPoint.Elements)
                        {
                            emptyObject[element.PropertyName.Trim()] = "";
                        }
                        
                        // Only add if the data point has elements
                        if (emptyObject.Count > 0)
                        {
                            emptyMapping.Add(emptyObject);
                            rootObject[dataPointName] = emptyMapping;
                        }
                    }
                }

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                return JsonSerializer.Serialize(rootObject, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting file mappings with all data points to JSON for file ID {FileId}", fileUploadId);
                throw;
            }
        }

        /// <summary>
        /// Sanitizes a filename to remove characters invalid for directory or file names.
        /// </summary>
        private static string SanitizeFileName(string name)
        {
            string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.\.+|[{0}]+)", invalidChars);
            string sanitized = Regex.Replace(name, invalidRegStr, "_");
            return string.IsNullOrWhiteSpace(sanitized) ? "_fallback_name_" : sanitized;
        }
    }
}