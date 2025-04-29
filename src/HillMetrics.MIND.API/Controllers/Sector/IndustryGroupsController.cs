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
    public class IndustryGroupsController(IHMediator mediator, IMapper mapper) : BaseHillMetricsController(mediator)
    {
        // GET Industry Groups
        [HttpGet]
        public async Task<IActionResult> GetAllIndustryGroups()
        {
            var result = await mediator.Send(new GetAllGicsIndustryGroupsQuery());

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return Ok(mapper.Map<IEnumerable<GicsIndustryGroupResponse>>(result.Value));
        }

        [HttpGet("industry-groups/{id}")]
        public async Task<IActionResult> GetIndustryGroupById(int id)
        {
            var result = await mediator.Send(new GetGicsIndustryGroupQuery(id));
            if (result.IsFailed) return NotFound(result.Errors.First().Message);

            return Ok(mapper.Map<GicsIndustryGroupResponse>(result.Value));
        }

        // POST Industry Groups
        [HttpPost]
        public async Task<IActionResult> CreateIndustryGroup([FromBody] GicsIndustryGroupRequest request)
        {
            var domain = mapper.Map<GicsIndustryGroup>(request);
            var command = new AddOrUpdateGicsIndustryGroupCommand(domain);

            await mediator.Send(command);

            return CreatedAtAction(nameof(GetAllIndustryGroups), request);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIndustryGroup(int id, [FromBody] GicsIndustryGroupRequest request)
        {
            var domain = mapper.Map<GicsIndustryGroup>(request);
            domain.Id = id;

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
    }
}
