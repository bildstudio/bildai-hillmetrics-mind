using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Requests.TradingVenue;

public class EditTradingVenueRequest
{
    /// <summary>
    /// Name of the trading venue
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Legal entity name of the venue
    /// </summary>
    public string? LegalEntityName { get; set; }

    /// <summary>
    /// Acronym of the venue
    /// </summary>
    public string? Acronym { get; set; }

    /// <summary>
    /// Legal Entity Identifier
    /// </summary>
    public string? Lei { get; set; }

    /// <summary>
    /// City of the venue
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Flag indicating if this is the main trading venue for its country
    /// </summary>
    public bool? IsMain { get; set; }

    /// <summary>
    /// Trading hours to set for this venue
    /// </summary>
    public List<TradingHourRequestDto>? TradingHours { get; set; }
}

public class TradingHourRequestDto
{
    /// <summary>
    /// Day of the week
    /// </summary>
    public DayOfWeek DayOfWeek { get; set; }

    /// <summary>
    /// Opening time in hh:mm format
    /// </summary>
    public string OpenTime { get; set; } = null!;

    /// <summary>
    /// Closing time in hh:mm format
    /// </summary>
    public string CloseTime { get; set; } = null!;
}