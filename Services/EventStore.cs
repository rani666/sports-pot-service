using System.Collections.Concurrent;
using System.Text.Json;

namespace sports_pot_service.Services;

/// <summary>
/// Singleton in-memory store for live events fetched from the external feed.
/// Keyed by event id. All services inject this to read current event state.
/// </summary>
public class EventStore
{
    private readonly ConcurrentDictionary<string, JsonElement> _events = new();

    public long LastTimestamp { get; private set; }

    public IReadOnlyDictionary<string, JsonElement> Events => _events;

    public void Apply(JsonElement root)
    {
        if (!root.TryGetProperty("data", out var dataEl))
            return;

        foreach (var ev in dataEl.EnumerateArray())
        {
            var key = ev.TryGetProperty("id", out var id) ? id.ToString() : Guid.NewGuid().ToString();
            _events[key] = ev.Clone();
        }

        LastTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}