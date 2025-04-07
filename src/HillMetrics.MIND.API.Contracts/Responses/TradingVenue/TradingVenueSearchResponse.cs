using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.TradingVenue;

public class TradingVenueSearchResponse
{
    /// <summary>
    /// ID of the trading venue
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the trading venue
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Market Identifier Code (MIC)
    /// </summary>
    public string Mic { get; set; } = null!;

    /// <summary>
    /// Acronym
    /// </summary>
    public string? Acronym { get; set; }

    /// <summary>
    /// Country information
    /// </summary>
    public CountryResponse Country { get; set; } = null!;

    /// <summary>
    /// City information
    /// </summary>
    public CityResponse City { get; set; } = null!;

    /// <summary>
    /// Currency information
    /// </summary>
    public CurrencyResponse Currency { get; set; } = null!;

    /// <summary>
    /// Flag indicating if this is the main trading venue for its country
    /// </summary>
    public bool IsMain { get; set; }

    /// <summary>
    /// Is the trading venue active
    /// </summary>
    public bool IsActive { get; set; }
}