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

            return File(file.Data, ContentTypeMapper.GetMimeType(file.FluxContentType), file.Name);
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
