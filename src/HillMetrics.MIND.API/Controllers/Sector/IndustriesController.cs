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
    public class IndustriesController(IHMediator mediator, IMapper mapper) : BaseHillMetricsController(mediator)
    {
        // --- GicsIndustry Endpoints ---
        [HttpGet]
        public async Task<IActionResult> GetAllIndustries()
        {
            var result = await mediator.Send(new GetAllGicsIndustriesQuery());
            return Ok(mapper.Map<IEnumerable<GicsIndustryResponse>>(result.Value));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetIndustryById(int id)
        {
            var result = await mediator.Send(new GetGicsIndustryQuery(id));
            if (result.IsFailed) return NotFound(result.Errors.First().Message);

            return Ok(mapper.Map<GicsIndustryResponse>(result.Value));
        }

        [HttpPost]
        public async Task<IActionResult> CreateIndustry([FromBody] GicsIndustryRequest request)
        {
            var domain = mapper.Map<GicsIndustry>(request);
            var command = new AddOrUpdateGicsIndustryCommand(domain);

            await mediator.Send(command);

            return CreatedAtAction(nameof(GetIndustryById), new { id = domain.Id }, request);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIndustry(int id, [FromBody] GicsIndustryRequest request)
        {
            var domain = mapper.Map<GicsIndustry>(request);
            domain.Id = id;

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
    }
}
