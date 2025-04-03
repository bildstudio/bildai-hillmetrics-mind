using HillMetrics.MIND.API.SDK.V1;
using System.Text.Json;
using System.Text;
using Microsoft.JSInterop;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingExportService"/> class.
        /// </summary>
        /// <param name="mindApi">The MIND API client.</param>
        /// <param name="jsRuntime">The JavaScript runtime.</param>
        /// <param name="logger">The logger.</param>
        public MappingExportService(
            IMindAPI mindApi,
            IJSRuntime jsRuntime,
            ILogger<MappingExportService> logger)
        {
            _mindApi = mindApi;
            _jsRuntime = jsRuntime;
            _logger = logger;
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
                        DataType = e.PropertyDataType.ToString(),
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
                        DataType = elementValue.PropertyDataType.ToString(),
                        Value = elementValue.ExtractedValue
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
    }
}