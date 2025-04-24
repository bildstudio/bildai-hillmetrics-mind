namespace HillMetrics.MIND.API.Contracts.Responses;

public class PagedApiResponseBase<T>
{
    public PagedApiResponseBase(IEnumerable<T> data, long totalRecords)
    {
        Data = data;
        TotalRecords = totalRecords;
    }
    public IEnumerable<T> Data { get; set; }
    public long TotalRecords { get; set; }

    /// <summary>
    /// List of countries that have multiple trading venues marked as main
    /// </summary>
    public List<string> CountriesWithMultipleMainVenues { get; set; } = new List<string>();

    /// <summary>
    /// List of countries that don't have any trading venue marked as main
    /// </summary>
    public List<string> CountriesWithNoMainVenue { get; set; } = new List<string>();
}