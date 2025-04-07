using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.TradingVenue;

public class TradingVenueResponse
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
    /// Operating MIC
    /// </summary>
    public string OperatingMic { get; set; } = null!;

    /// <summary>
    /// MIC type
    /// </summary>
    public string MicType { get; set; } = null!;

    /// <summary>
    /// Legal entity name
    /// </summary>
    public string? LegalEntityName { get; set; }

    /// <summary>
    /// Acronym
    /// </summary>
    public string? Acronym { get; set; }

    /// <summary>
    /// Legal Entity Identifier
    /// </summary>
    public string? Lei { get; set; }

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
    /// Market category code
    /// </summary>
    public string? MarketCategoryCode { get; set; }

    /// <summary>
    /// Flag indicating if this is the main trading venue for its country
    /// </summary>
    public bool IsMain { get; set; }

    /// <summary>
    /// Trading hours
    /// </summary>
    public List<TradingHourResponse> TradingHours { get; set; } = new List<TradingHourResponse>();
}

public class CountryResponse
{
    /// <summary>
    /// ISO 3166-1 alpha-2 country code
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// Country name
    /// </summary>
    public string Name { get; set; } = null!;
}

public class CityResponse
{
    /// <summary>
    /// City name
    /// </summary>
    public string Name { get; set; } = null!;
}

public class CurrencyResponse
{
    /// <summary>
    /// ISO 4217 currency code
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// Currency name
    /// </summary>
    public string Name { get; set; } = null!;
}

public class TradingHourResponse
{
    /// <summary>
    /// Day of the week
    /// </summary>
    public DayOfWeek DayOfWeek { get; set; }

    /// <summary>
    /// Opening time
    /// </summary>
    public TimeSpan OpenTime { get; set; }

    /// <summary>
    /// Closing time
    /// </summary>
    public TimeSpan CloseTime { get; set; }

    /// <summary>
    /// Formatted opening time (HH:mm)
    /// </summary>
    public string OpenTimeFormatted => OpenTime.ToString(@"hh\:mm");

    /// <summary>
    /// Formatted closing time (HH:mm)
    /// </summary>
    public string CloseTimeFormatted => CloseTime.ToString(@"hh\:mm");
}