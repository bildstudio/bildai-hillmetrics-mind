using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Requests.TradingVenue;

public class GetTradingVenueRequest
{
    /// <summary>
    /// ID of the trading venue
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Market Identifier Code (MIC) of the trading venue
    /// </summary>
    public string? Mic { get; set; }
}