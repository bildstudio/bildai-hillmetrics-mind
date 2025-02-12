namespace HillMetrics.MIND.API.Contracts.Responses;
public class ApiResponseBase<T>(T data)
{
    public T Data { get; set; } = data;
}

public class PagedApiResponseBase<T>
{
    public PagedApiResponseBase(IEnumerable<T> data, long totalRecords)
    {
        Data = data;
        TotalRecords = totalRecords;
    }
    public IEnumerable<T> Data { get; set; }
    public long TotalRecords { get; set; }
}