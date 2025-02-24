using AutoMapper;
using HillMetrics.Core.API.Responses;
using HillMetrics.Core.Financial.Gics;
using HillMetrics.MIND.API.Contracts.Responses.Sector;
using HillMetrics.Normalized.Domain.Contracts.Sector;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HillMetrics.MIND.API.Controllers
{
    [Route("api/v{v:apiVersion}/[controller]"), AllowAnonymous]
    public class SectorController(IMediator mediator, IMapper mapper) : BaseHillMetricsController(mediator)
    {
        // GET Sectors
        [HttpGet("sectors")]
        public async Task<IActionResult> GetAllSectors()
        {
            var result = await mediator.Send(new GetAllGicsSectorsQuery());

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return Ok(mapper.Map<IEnumerable<GicsSectorResponse>>(result.Value));
        }

        [HttpGet("sectors/{id}")]
        public async Task<IActionResult> GetSectorById(int id)
        {
            var result = await mediator.Send(new GetGicsSectorQuery(id));

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return Ok(mapper.Map<GicsSectorResponse>(result.Value));
        }

        // POST Sectors
        [HttpPost("sectors")]
        public async Task<IActionResult> CreateSector([FromBody] GicsSectorRequest request)
        {
            var domain = mapper.Map<GicsSector>(request);
            var command = new AddOrUpdateGicsSectorCommand(domain);

            await mediator.Send(command);

            return CreatedAtAction(nameof(GetSectorById), new { id = domain.Id }, request);
        }

        // GET Industry Groups
        [HttpGet("industry-groups")]
        public async Task<IActionResult> GetAllIndustryGroups()
        {
            var result = await mediator.Send(new GetAllGicsIndustryGroupsQuery());
            
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return Ok(mapper.Map<IEnumerable<GicsIndustryGroupResponse>>(result.Value));
        }

        // POST Industry Groups
        [HttpPost("industry-groups")]
        public async Task<IActionResult> CreateIndustryGroup([FromBody] GicsIndustryGroupRequest request)
        {
            var domain = mapper.Map<GicsIndustryGroup>(request);
            var command = new AddOrUpdateGicsIndustryGroupCommand(domain);

            await mediator.Send(command);

            return CreatedAtAction(nameof(GetAllIndustryGroups), request);
        }

        [HttpPost("sectors")]
        public async Task<IActionResult> CreateOrUpdateSector([FromBody] GicsSectorRequest request)
        {
            var domain = mapper.Map<GicsSector>(request);
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

        [HttpGet("industry-groups/{id}")]
        public async Task<IActionResult> GetIndustryGroupById(int id)
        {
            var result = await mediator.Send(new GetGicsIndustryGroupQuery(id));
            if (result.IsFailed) return NotFound(result.Errors.First().Message);

            return Ok(mapper.Map<GicsIndustryGroupResponse>(result.Value));
        }

        [HttpPost("industry-groups")]
        public async Task<IActionResult> CreateOrUpdateIndustryGroup([FromBody] GicsIndustryGroupRequest request)
        {
            var domain = mapper.Map<GicsIndustryGroup>(request);
            var command = new AddOrUpdateGicsIndustryGroupCommand(domain);

            await mediator.Send(command);

            return CreatedAtAction(nameof(GetIndustryGroupById), new { id = domain.Id }, request);
        }

        //[HttpDelete("industry-groups/{id}")]
        //public async Task<IActionResult> DeleteIndustryGroup(int id)
        //{
        //    await mediator.Send(new DeleteGicsIndustryGroupCommand(id));
        //    return NoContent();
        //}

        // --- GicsIndustry Endpoints ---
        [HttpGet("industries")]
        public async Task<IActionResult> GetAllIndustries()
        {
            var result = await mediator.Send(new GetAllGicsIndustriesQuery());
            return Ok(mapper.Map<IEnumerable<GicsIndustryResponse>>(result.Value));
        }

        [HttpGet("industries/{id}")]
        public async Task<IActionResult> GetIndustryById(int id)
        {
            var result = await mediator.Send(new GetGicsIndustryQuery(id));
            if (result.IsFailed) return NotFound(result.Errors.First().Message);

            return Ok(mapper.Map<GicsIndustryResponse>(result.Value));
        }

        [HttpPost("industries")]
        public async Task<IActionResult> CreateOrUpdateIndustry([FromBody] GicsIndustryRequest request)
        {
            var domain = mapper.Map<GicsIndustry>(request);
            var command = new AddOrUpdateGicsIndustryCommand(domain);

            await mediator.Send(command);

            return CreatedAtAction(nameof(GetIndustryById), new { id = domain.Id }, request);
        }

        //[HttpDelete("industries/{id}")]
        //public async Task<IActionResult> DeleteIndustry(int id)
        //{
        //    await mediator.Send(new DeleteGicsIndustryCommand(id));
        //    return NoContent();
        //}

        // --- GicsSubIndustry Endpoints ---
        [HttpGet("sub-industries")]
        public async Task<IActionResult> GetAllSubIndustries()
        {
            var result = await mediator.Send(new GetAllGicsSubIndustriesQuery());
            return Ok(mapper.Map<IEnumerable<GicsSubIndustryResponse>>(result.Value));
        }

        [HttpGet("sub-industries/{id}")]
        public async Task<IActionResult> GetSubIndustryById(int id)
        {
            var result = await mediator.Send(new GetGicsSubIndustryQuery(id));
            if (result.IsFailed) return NotFound(result.Errors.First().Message);

            return Ok(mapper.Map<GicsSubIndustryResponse>(result.Value));
        }

        [HttpPost("sub-industries")]
        public async Task<IActionResult> CreateOrUpdateSubIndustry([FromBody] GicsSubIndustryRequest request)
        {
            var domain = mapper.Map<GicsSubIndustry>(request);
            var command = new AddOrUpdateGicsSubIndustryCommand(domain);

            await mediator.Send(command);

            return CreatedAtAction(nameof(GetSubIndustryById), new { id = domain.Id }, request);
        }

        //[HttpDelete("sub-industries/{id}")]
        //public async Task<IActionResult> DeleteSubIndustry(int id)
        //{
        //    await mediator.Send(new DeleteGicsSubIndustryCommand(id));
        //    return NoContent();
        //}
    }
}
