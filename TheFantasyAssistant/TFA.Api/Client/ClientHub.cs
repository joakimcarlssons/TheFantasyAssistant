using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace TFA.Api.Client;

public class ClientHub : Hub
{
    private static readonly ConcurrentDictionary<string, string> _connections = [];

    public string Connect() => Context.ConnectionId;

    public override Task OnConnectedAsync()
    {
        _connections.TryAdd(Context.ConnectionId, Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _connections.TryRemove(Context.ConnectionId, out _);
        return base.OnDisconnectedAsync(exception);
    }

    public static IEnumerable<string> GetConnections()
    {
        foreach (string connection in _connections.Keys)
        {
            yield return connection;
        }
    }
}
