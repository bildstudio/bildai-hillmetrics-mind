using HillMetrics.Core.Blazor.AuthModule.Services;
using HillMetrics.MIND.FrontApp.Configs;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace HillMetrics.MIND.FrontApp.Services;

public class SignalRNotificationService : ISignalRNotificationService
{
    private readonly IOptions<ServicesSettings> _serviceOptions;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SignalRNotificationService> _logger;

    private HubConnection? _hubConnection;
    private readonly ConcurrentDictionary<string, List<Func<object, Task>>> _topicSubscriptions = new();
    private readonly ConcurrentDictionary<string, bool> _serverSubscribedTopics = new(); // Track server-side subscriptions
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private bool _isDisposed = false;
    private volatile bool _isConnected = false; // Use volatile for thread safety on read/write

    public bool IsConnected => _isConnected;
    public event Action<bool>? ConnectionStatusChanged;

    public SignalRNotificationService(
        IOptions<ServicesSettings> serviceOptions,
        IServiceProvider serviceProvider,
        ILogger<SignalRNotificationService> logger)
    {
        _serviceOptions = serviceOptions;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task EnsureInitializedAsync()
    {
        if (_hubConnection == null || _hubConnection.State == HubConnectionState.Disconnected)
        {
            await _connectionLock.WaitAsync();
            try
            {
                // Double-check after acquiring the lock
                if (_hubConnection == null || _hubConnection.State == HubConnectionState.Disconnected)
                {
                    _logger.LogInformation("Initializing SignalR connection...");
                    await InitializeAndStartConnectionAsync();
                }
            }
            finally
            {
                _connectionLock.Release();
            }
        }
    }

    private async Task InitializeAndStartConnectionAsync()
    {
        if (_isDisposed) throw new ObjectDisposedException(nameof(SignalRNotificationService));

        var hubUrl = $"{_serviceOptions.Value.SignalRApi}/notification-hub"; // Central hub
        _logger.LogInformation("Attempting to connect to SignalR hub at: {HubUrl}", hubUrl);

        string? accessToken = string.Empty;
        try
        {
            // Create a scope to resolve the scoped IAuthService
            using var scope = _serviceProvider.CreateScope();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
            accessToken = await authService.GetAccessTokenAsync();

            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogWarning("SignalR connection requires authentication, but no access token was retrieved.");
                // Decide if connection should proceed without token or fail
                // For now, let's proceed but log prominently
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get access token for SignalR connection.");
            // Decide if connection should fail here
        }


        _hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                if (!string.IsNullOrEmpty(accessToken))
                {
                    options.AccessTokenProvider = () => Task.FromResult<string?>(accessToken);
                }
            })
            .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30) })
            .Build();

        _hubConnection.On<string, object>("onTopicNotification", HandleNotification);

        _hubConnection.Reconnected += OnReconnected;
        _hubConnection.Closed += OnClosed;

        try
        {
            await _hubConnection.StartAsync();
            _isConnected = true;
            ConnectionStatusChanged?.Invoke(true);
            _logger.LogInformation("SignalR connection established successfully. Connection ID: {ConnectionId}", _hubConnection.ConnectionId);

            // Re-subscribe to server topics if any were tracked before potential disconnect
            await ResubscribeToServerTopics();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SignalR connection failed to start.");
            _isConnected = false;
            ConnectionStatusChanged?.Invoke(false);
            // Optionally dispose connection here if start fails definitively
            await DisposeCoreAsync(); // Clean up resources if start fails
        }
    }

    public async Task SubscribeAsync(string topic, Func<object, Task> handler)
    {
        if (_isDisposed) throw new ObjectDisposedException(nameof(SignalRNotificationService));

        await EnsureInitializedAsync(); // Ensure connection is ready

        _topicSubscriptions.AddOrUpdate(topic,
            addValueFactory: _ => new List<Func<object, Task>> { handler },
            updateValueFactory: (_, existingList) =>
            {
                lock (existingList) // Lock the specific list for modification
                {
                    if (!existingList.Contains(handler))
                    {
                        existingList.Add(handler);
                    }
                }
                return existingList;
            });

        // Subscribe on the server if this is the first client handler for this topic
        await SubscribeToServerTopicIfNeeded(topic);

        _logger.LogDebug("Handler subscribed to topic '{Topic}' client-side.", topic);
    }

    private async Task SubscribeToServerTopicIfNeeded(string topic)
    {
        // Check if we're already tracking a server subscription for this topic
        if (!_serverSubscribedTopics.ContainsKey(topic))
        {
            if (_hubConnection != null && _hubConnection.State == HubConnectionState.Connected)
            {
                try
                {
                    _logger.LogInformation("Attempting to subscribe to topic '{Topic}' on server hub.", topic);
                    await _hubConnection.InvokeAsync("Subscribe", topic);
                    _serverSubscribedTopics.TryAdd(topic, true); // Track server subscription
                    _logger.LogInformation("Successfully subscribed to topic '{Topic}' on server hub.", topic);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to subscribe to topic '{Topic}' on server hub.", topic);
                    // Optionally remove topic from _serverSubscribedTopics if subscription failed
                    _serverSubscribedTopics.TryRemove(topic, out _);
                }
            }
            else
            {
                _logger.LogWarning("Cannot subscribe to server topic '{Topic}' yet, connection not established. Will attempt on reconnect.", topic);
                // We don't add to _serverSubscribedTopics yet, it will be attempted on connect/reconnect
            }
        }
    }

    public Task UnsubscribeAsync(string topic, Func<object, Task> handler)
    {
        if (_isDisposed) return Task.CompletedTask;

        if (_topicSubscriptions.TryGetValue(topic, out var handlers))
        {
            lock (handlers) // Lock the specific list for modification
            {
                handlers.Remove(handler);
            }
            _logger.LogDebug("Handler unsubscribed from topic '{Topic}' client-side.", topic);

            // Optional: If handlers list becomes empty, consider unsubscribing from server hub.
            // Requires careful management if multiple instances/components use the service.
            // For simplicity, we won't auto-unsubscribe from server for now.
            // if (handlers.Count == 0) { /* await UnsubscribeFromServerTopic(topic); */ }
        }
        return Task.CompletedTask;
    }

    private void HandleNotification(string topic, object message)
    {
        _logger.LogTrace("Received notification on topic '{Topic}'.", topic);
        if (_topicSubscriptions.TryGetValue(topic, out var handlers))
        {
            List<Func<object, Task>> handlersSnapshot;
            lock (handlers) // Lock while creating snapshot to avoid collection modified issues
            {
                handlersSnapshot = new List<Func<object, Task>>(handlers);
            }

            _logger.LogDebug("Found {HandlerCount} handlers for topic '{Topic}'. Invoking...", handlersSnapshot.Count, topic);
            foreach (var handler in handlersSnapshot)
            {
                try
                {
                    // Don't await here directly to avoid blocking the SignalR thread
                    // Let each handler run independently. Consider Task.Run if handlers are CPU-bound.
                    _ = handler(message).ContinueWith(t => {
                        if (t.IsFaulted)
                        {
                            _logger.LogError(t.Exception, "Error executing SignalR handler for topic '{Topic}'.", topic);
                        }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error invoking handler for topic '{Topic}'.", topic);
                }
            }
        }
        else
        {
            _logger.LogDebug("No client handlers registered for received topic '{Topic}'.", topic);
        }
    }

    private Task OnReconnected(string? connectionId)
    {
        _logger.LogInformation("SignalR connection re-established. Connection ID: {ConnectionId}", connectionId);
        _isConnected = true;
        ConnectionStatusChanged?.Invoke(true);
        // Re-subscribe to all topics previously subscribed to on the server
        return ResubscribeToServerTopics();
    }

    private Task OnClosed(Exception? error)
    {
        _logger.LogWarning(error, "SignalR connection closed.");
        _isConnected = false;
        ConnectionStatusChanged?.Invoke(false);
        // Do not clear _serverSubscribedTopics here, retain them for reconnection attempts
        return Task.CompletedTask;
    }

    private async Task ResubscribeToServerTopics()
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            // Create a snapshot of topics we intended to subscribe to
            var topicsToResubscribe = _serverSubscribedTopics.Keys.ToList();
            if (topicsToResubscribe.Any())
            {
                _logger.LogInformation("Attempting to re-subscribe to {Count} server topics after (re)connection.", topicsToResubscribe.Count);
                foreach (var topic in topicsToResubscribe)
                {
                    try
                    {
                        await _hubConnection.InvokeAsync("Subscribe", topic);
                        _logger.LogInformation("Successfully re-subscribed to topic '{Topic}' on server hub.", topic);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to re-subscribe to topic '{Topic}' on server hub.", topic);
                        // Keep topic in _serverSubscribedTopics, maybe next reconnect will succeed.
                    }
                }
            }
        }
        else
        {
            _logger.LogWarning("Cannot re-subscribe to server topics, connection not available.");
        }
    }


    public async ValueTask DisposeAsync()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        await DisposeCoreAsync();

        _connectionLock.Dispose();
        GC.SuppressFinalize(this);
    }

    private async Task DisposeCoreAsync()
    {
        if (_hubConnection != null)
        {
            _hubConnection.Reconnected -= OnReconnected;
            _hubConnection.Closed -= OnClosed;

            // Unsubscribe from all server topics before disposing
            var topicsToUnsubscribe = _serverSubscribedTopics.Keys.ToList();
            if (_hubConnection.State == HubConnectionState.Connected && topicsToUnsubscribe.Any())
            {
                _logger.LogInformation("Unsubscribing from {Count} server topics during disposal.", topicsToUnsubscribe.Count);
                foreach (var topic in topicsToUnsubscribe)
                {
                    try
                    {
                        await _hubConnection.InvokeAsync("Unsubscribe", topic);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error unsubscribing from server topic '{Topic}' during disposal.", topic);
                    }
                }
            }

            try
            {
                await _hubConnection.StopAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping SignalR connection during disposal.");
            }
            finally
            {
                await _hubConnection.DisposeAsync();
                _hubConnection = null;
                _isConnected = false;
                ConnectionStatusChanged?.Invoke(false);
                _logger.LogInformation("SignalR connection disposed.");
            }
        }
        _topicSubscriptions.Clear();
        _serverSubscribedTopics.Clear();
    }
}