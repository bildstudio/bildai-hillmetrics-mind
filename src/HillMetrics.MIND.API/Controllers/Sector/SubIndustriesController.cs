using AutoMapper;
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
    public class SubIndustriesController(IHMediator mediator, IMapper mapper) : BaseHillMetricsController(mediator)
    {
        // --- GicsSubIndustry Endpoints ---
        [HttpGet]
        public async Task<IActionResult> GetAllSubIndustries()
        {
            var result = await mediator.Send(new GetAllGicsSubIndustriesQuery());
            return Ok(mapper.Map<IEnumerable<GicsSubIndustryResponse>>(result.Value));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubIndustryById(int id)
        {
            var result = await mediator.Send(new GetGicsSubIndustryQuery(id));
            if (result.IsFailed) return NotFound(result.Errors.First().Message);

            return Ok(mapper.Map<GicsSubIndustryResponse>(result.Value));
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubIndustry([FromBody] GicsSubIndustryRequest request)
        {
            var domain = mapper.Map<GicsSubIndustry>(request);
            var command = new AddOrUpdateGicsSubIndustryCommand(domain);

            await mediator.Send(command);

            return CreatedAtAction(nameof(GetSubIndustryById), new { id = domain.Id }, request);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubIndustry(int id, [FromBody] GicsSubIndustryRequest request)
        {
            var domain = mapper.Map<GicsSubIndustry>(request);
            domain.Id = id;

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
