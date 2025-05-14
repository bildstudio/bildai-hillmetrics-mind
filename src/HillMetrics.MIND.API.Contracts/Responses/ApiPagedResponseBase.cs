namespace HillMetrics.MIND.API.Contracts.Responses;

public class ApiPagedResponseBase<T>
{
    public ApiPagedResponseBase(IEnumerable<T> data, long totalRecords)
    {
        Data = data;
        TotalRecords = totalRecords;
    }
    public IEnumerable<T> Data { get; set; }
    public long TotalRecords { get; set; }
}