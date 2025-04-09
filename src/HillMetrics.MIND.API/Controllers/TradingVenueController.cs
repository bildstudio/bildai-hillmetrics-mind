using AutoMapper;
using HillMetrics.Core.API.Responses;
using HillMetrics.MIND.API.Contracts.Requests.TradingVenue;
using HillMetrics.MIND.API.Contracts.Responses;
using HillMetrics.MIND.API.Contracts.Responses.TradingVenue;
using HillMetrics.Normalized.Domain.Contracts.Market.Cqrs.TradingVenue;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HillMetrics.MIND.API.Controllers
{
    [Route("api/v{v:apiVersion}/[controller]")]
    public class TradingVenueController : BaseHillMetricsController
    {
        private readonly ILogger<TradingVenueController> _logger;

        public TradingVenueController(
            IMediator mediator,
            ILogger<TradingVenueController> logger) : base(mediator)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get a trading venue by ID or MIC
        /// </summary>
        /// <param name="request">Request with either ID or MIC</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Trading venue details</returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponseBase<TradingVenueResponse>>> GetTradingVenue(
            [FromQuery] GetTradingVenueRequest request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting trading venue with ID: {Id} or MIC: {Mic}", request.Id, request.Mic);

            var query = new GetTradingVenueQuery
            {
                Id = request.Id,
                Mic = request.Mic
            };

            var result = await Mediator.Send(query, cancellationToken);

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            var response = MapTradingVenueToResponse(result.Value);
            return new ApiResponseBase<TradingVenueResponse>(response);
        }

        /// <summary>
        /// Search for trading venues based on criteria
        /// </summary>
        /// <param name="request">Search criteria</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paged list of trading venues</returns>
        [HttpGet("search")]
        public async Task<ActionResult<PagedApiResponseBase<TradingVenueSearchResponse>>> SearchTradingVenues(
            [FromQuery] SearchTradingVenueRequest request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Searching trading venues with criteria: {Criteria}",
                System.Text.Json.JsonSerializer.Serialize(request));

            var query = MapSearchRequestToQuery(request);
            var result = await Mediator.Send(query, cancellationToken);

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            var responses = result.Value.TradingVenues.Select(MapTradingVenueToSearchResponse).ToList();
            return new PagedApiResponseBase<TradingVenueSearchResponse>(responses, result.Value.TotalCount);
        }

        /// <summary>
        /// Edit a trading venue
        /// </summary>
        /// <param name="id">ID of the trading venue to edit</param>
        /// <param name="request">Edit request with updated values</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated trading venue details</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseBase<TradingVenueResponse>>> EditTradingVenue(
            int id,
            [FromBody] EditTradingVenueRequest request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Editing trading venue with ID: {Id}", id);

            var command = MapEditRequestToCommand(id, request);
            var result = await Mediator.Send(command, cancellationToken);

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            var response = MapTradingVenueToResponse(result.Value.TradingVenue);
            return new ApiResponseBase<TradingVenueResponse>(response);
        }

        #region Manual Mapping Methods

        private SearchTradingVenueQuery MapSearchRequestToQuery(SearchTradingVenueRequest request)
        {
            return new SearchTradingVenueQuery
            {
                SearchTerm = request.SearchTerm,
                CountryCode = request.CountryCode,
                OnlyMain = request.OnlyMain,
                Pagination = request.Pagination,
                Sorting = request.Sorting
            };
        }

        private EditTradingVenueCommand MapEditRequestToCommand(int id, EditTradingVenueRequest request)
        {
            return new EditTradingVenueCommand
            {
                Id = id,
                Name = request.Name,
                LegalEntityName = request.LegalEntityName,
                Acronym = request.Acronym,
                Lei = request.Lei,
                City = request.City,
                IsMain = request.IsMain,
                TradingHours = request.TradingHours?.Select(th => new TradingHourDto
                {
                    DayOfWeek = th.DayOfWeek,
                    OpenTime = th.OpenTime,
                    CloseTime = th.CloseTime
                }).ToList()
            };
        }

        private TradingVenueResponse MapTradingVenueToResponse(HillMetrics.Normalized.Domain.Contracts.Market.TradingVenue venue)
        {
            return new TradingVenueResponse
            {
                Id = venue.Id,
                Name = venue.Name,
                Mic = venue.Mic,
                OperatingMic = venue.OperatingMic,
                MicType = venue.MicType.ToString(),
                LegalEntityName = venue.LegalEntityName,
                Acronym = venue.Acronym,
                Lei = venue.Lei,
                Country = new CountryResponse
                {
                    Code = venue.Country.Code,
                    Name = venue.Country.Name
                },
                City = new CityResponse
                {
                    Name = venue.City.Name
                },
                Currency = new CurrencyResponse
                {
                    Code = venue.Currency.Code,
                    Name = venue.Currency.Name
                },
                MarketCategoryCode = venue.MarketCategoryCode,
                IsMain = venue.IsMain,
                TradingHours = venue.TradingHours.Select(th => new TradingHourResponse
                {
                    DayOfWeek = th.DayOfWeek,
                    OpenTime = th.OpenTime,
                    CloseTime = th.CloseTime
                }).ToList()
            };
        }

        private TradingVenueSearchResponse MapTradingVenueToSearchResponse(HillMetrics.Normalized.Domain.Contracts.Market.TradingVenue venue)
        {
            return new TradingVenueSearchResponse
            {
                Id = venue.Id,
                Name = venue.Name,
                Mic = venue.Mic,
                Acronym = venue.Acronym,
                Country = new CountryResponse
                {
                    Code = venue.Country.Code,
                    Name = venue.Country.Name
                },
                City = new CityResponse
                {
                    Name = venue.City.Name
                },
                Currency = new CurrencyResponse
                {
                    Code = venue.Currency.Code,
                    Name = venue.Currency.Name
                },
                IsMain = venue.IsMain,
            };
        }

        #endregion
    }
}
