//using Asp.Versioning;

namespace HillMetrics.MIND.API.Tests;

public interface IConsumerDataSingleton
{
    string Message { get; set; }
}

public class ConsumerDataSingleton : IConsumerDataSingleton
{
    public string Message { get; set; } = string.Empty;
}
