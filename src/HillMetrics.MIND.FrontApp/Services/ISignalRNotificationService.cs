namespace HillMetrics.MIND.FrontApp.Services;

public interface ISignalRNotificationService : IAsyncDisposable
{
    /// <summary>
    /// Gets a value indicating whether the SignalR connection is currently active.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Event triggered when the connection status changes.
    /// </summary>
    event Action<bool>? ConnectionStatusChanged;

    /// <summary>
    /// Ensures the SignalR connection is initialized and started.
    /// Can be called multiple times; initializes only once.
    /// </summary>
    Task EnsureInitializedAsync();

    /// <summary>
    /// Subscribes a handler to receive messages for a specific topic from the SignalR hub.
    /// Also subscribes to the topic on the server hub if not already subscribed.
    /// </summary>
    /// <param name="topic">The topic name to subscribe to.</param>
    /// <param name="handler">The asynchronous handler function to execute when a message is received. Takes the message object as input.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SubscribeAsync(string topic, Func<object, Task> handler);

    /// <summary>
    /// Unsubscribes a specific handler from a topic.
    /// Does not automatically unsubscribe from the topic on the server hub (to avoid issues if other handlers still exist).
    /// </summary>
    /// <param name="topic">The topic name.</param>
    /// <param name="handler">The specific handler function to remove.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UnsubscribeAsync(string topic, Func<object, Task> handler);
}