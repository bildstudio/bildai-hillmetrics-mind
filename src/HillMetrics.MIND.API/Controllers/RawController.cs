using AutoMapper;
using HillMetrics.Core.Common;
using HillMetrics.Core.Mediator;
using HillMetrics.Normalized.Domain.Contracts.Repository;
using HillMetrics.Raw.Infrastructure.Contracts.Repository;
using HillMetrics.Raw.Infrastructure.Database.Repository;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HillMetrics.MIND.API.Controllers
{
    [Route("api/v{v:apiVersion}/[controller]")]
    public class RawController(IHMediator mediator, IMapper mapper, IRawFluxDataRepository rawFluxDataRepository, IFileMetadataRepository fileMetadataRepository) : BaseHillMetricsController(mediator)
    {
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFile(string id)
        {
            var file = await rawFluxDataRepository.GetAsync(id, CancellationToken.None);

            if(file == null)
            {
                return NotFound();
            }

            // For large files, use streaming instead of File() method
            if (file.Data.CanSeek && file.Data.Length > 10 * 1024 * 1024) // 10MB threshold
            {
                var contentType = ContentTypeMapper.GetMimeType(file.FluxContentType);
                Response.Headers.Add("Content-Length", file.Data.Length.ToString());
                return new FileStreamResult(file.Data, contentType) { FileDownloadName = file.Name };
            }

            return File(file.Data, ContentTypeMapper.GetMimeType(file.FluxContentType), file.Name);
        }

        /// <summary>
        /// Get the size of a file
        /// </summary>
        /// <param name="id">File ID</param>
        /// <returns>File size in bytes</returns>
        [HttpGet("size/{id}")]
        public async Task<IActionResult> GetFileSize(string id)
        {
            var file = await rawFluxDataRepository.GetAsync(id, CancellationToken.None);

            if (file == null)
            {
                return NotFound();
            }

            try
            {
                long fileSize = 0;

                if (file.Data.CanSeek)
                {
                    // Si le stream supporte Seek, utiliser Length
                    fileSize = file.Data.Length;
                }
                else
                {
                    // Sinon, copier dans un MemoryStream pour obtenir la taille
                    using (var ms = new MemoryStream())
                    {
                        var originalPosition = file.Data.Position;
                        file.Data.Seek(0, SeekOrigin.Begin);
                        await file.Data.CopyToAsync(ms);
                        fileSize = ms.Length;
                        file.Data.Seek(originalPosition, SeekOrigin.Begin); // Restaurer la position
                    }
                }

                return Ok(new {
                    FileId = id,
                    FileName = file.Name,
                    SizeInBytes = fileSize,
                    SizeFormatted = FormatFileSize(fileSize)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error getting file size: {ex.Message}");
            }
        }

        /// <summary>
        /// Format file size in human readable format
        /// </summary>
        /// <param name="sizeInBytes">Size in bytes</param>
        /// <returns>Formatted size string</returns>
        private static string FormatFileSize(long sizeInBytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int suffixIndex = 0;
            double size = sizeInBytes;

            while (size >= 1024 && suffixIndex < suffixes.Length - 1)
            {
                size /= 1024;
                suffixIndex++;
            }

            return $"{size:N2} {suffixes[suffixIndex]}";
        }

        /// <summary>
        /// Download a file from FileMetadataRepository using its ID
        /// </summary>
        /// <param name="id">File metadata ID</param>
        /// <returns>File stream for download</returns>
        [HttpGet("stored-file/{id:int}")]
        public async Task<IActionResult> DownloadStoredFile(int id)
        {
            var result = await fileMetadataRepository.DownloadStreamAsync(id, CancellationToken.None);

            if (result.IsFailed)
            {
                return NotFound($"File not found: {result.Errors.FirstOrDefault()?.Message}");
            }

            var (stream, metadata) = result.Value;

            // Retourner le fichier avec le bon type MIME et nom
            return File(
                stream,
                metadata.ContentType,
                metadata.FileName
            );
        }
    }
}
