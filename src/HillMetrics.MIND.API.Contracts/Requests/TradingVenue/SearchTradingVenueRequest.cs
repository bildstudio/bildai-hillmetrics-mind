using HillMetrics.Core.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Requests.TradingVenue;

public class SearchTradingVenueRequest
{
    /// <summary>
    /// Optional search term to filter trading venues by name, MIC, or acronym
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Optional country code to filter trading venues by country
    /// </summary>
    public string? CountryCode { get; set; }

    /// <summary>
    /// Include only main trading venues per country if true
    /// </summary>
    public bool? OnlyMain { get; set; }

    /// <summary>
    /// Pagination settings
    /// </summary>
    public Pagination Pagination { get; set; } = Pagination.Default;

    /// <summary>
    /// Sorting options
    /// </summary>
    public Sorting Sorting { get; set; } = new Sorting("Mic", SortDirection.Ascending);
}