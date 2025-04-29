using AutoMapper;
using HillMetrics.Core.API.Responses;
using HillMetrics.Core.Financial.Gics;
using HillMetrics.Core.Mediator;
using HillMetrics.MIND.API.Contracts.Responses.Sector;
using HillMetrics.Normalized.Domain.Contracts.Sector;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HillMetrics.MIND.API.Controllers.Sector
{
    [Route("api/v{v:apiVersion}/[controller]")]
    public class SectorController(IHMediator mediator, IMapper mapper) : BaseHillMetricsController(mediator)
    {
        // GET Sectors
        [HttpGet]
        public async Task<IActionResult> GetAllSectors()
        {
            var result = await mediator.Send(new GetAllGicsSectorsQuery());

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return Ok(mapper.Map<IEnumerable<GicsSectorResponse>>(result.Value));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSectorById(int id)
        {
            var result = await mediator.Send(new GetGicsSectorQuery(id));

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return Ok(mapper.Map<GicsSectorResponse>(result.Value));
        }

        // POST Sectors
        [HttpPost]
        public async Task<IActionResult> CreateSector([FromBody] GicsSectorRequest request)
        {
            var domain = mapper.Map<GicsSector>(request);
            var command = new AddOrUpdateGicsSectorCommand(domain);

            await mediator.Send(command);

            return CreatedAtAction(nameof(GetSectorById), new { id = domain.Id }, request);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSector(int id, GicsSectorRequest request)
        {
            var domain = mapper.Map<GicsSector>(request);
            domain.Id = id;

            var command = new AddOrUpdateGicsSectorCommand(domain);

            await mediator.Send(command);

            return CreatedAtAction(nameof(GetSectorById), new { id = domain.Id }, request);
        }

        //[HttpDelete("sectors/{id}")]
        //public async Task<IActionResult> DeleteSector(int id)
        //{
        //    await mediator.Send(new DeleteGicsSectorCommand(id));
        //    return NoContent();
        //}
    }
}
