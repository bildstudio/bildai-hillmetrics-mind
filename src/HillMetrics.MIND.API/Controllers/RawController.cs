using AutoMapper;
using HillMetrics.Core.Common;
using HillMetrics.Raw.Infrastructure.Contracts.Repository;
using HillMetrics.Raw.Infrastructure.Database.Repository;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HillMetrics.MIND.API.Controllers
{
    [Route("api/v{v:apiVersion}/[controller]")]
    public class RawController(IMediator mediator, IMapper mapper, IRawFluxDataRepository rawFluxDataRepository) : BaseHillMetricsController(mediator)
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
    }
}
