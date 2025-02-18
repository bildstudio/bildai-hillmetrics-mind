namespace HillMetrics.MIND.API.Contracts.Responses;
public class ApiResponseBase<T>(T data)
{
    public T Data { get; set; } = data;
}
