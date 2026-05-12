using System.Text.Json;

namespace sports_pot_service.Services;

public class EventSyncService : BackgroundService
{
    private const string BaseUrl = "https://q.idobet.com/services/evapi/event/GetEvents";
    private const int SportTypeId = 31;
    private const int PollIntervalMs = 2000;

    private readonly HttpClient _http;
    private readonly EventStore _store;
    private readonly ILogger<EventSyncService> _logger;

    public EventSyncService(IHttpClientFactory factory, EventStore store, ILogger<EventSyncService> logger)
    {
        _http = factory.CreateClient("events");
        _store = store;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await LoadInitialAsync(ct);

        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(PollIntervalMs));

        while (await timer.WaitForNextTickAsync(ct))
        {
            await PollUpdateAsync(ct);
        }
    }

    private async Task LoadInitialAsync(CancellationToken ct)
    {
        try
        {
            var url = $"{BaseUrl}?sportTypeIds={SportTypeId}";
            var json = await _http.GetStringAsync(url, ct);
            var root = JsonDocument.Parse(json).RootElement;
            _store.Apply(root);
            _logger.LogInformation("Events loaded. timestamp={Timestamp} count={Count}",
                _store.LastTimestamp, _store.Events.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load initial events.");
        }
    }

    private async Task PollUpdateAsync(CancellationToken ct)
    {
        try
        {
            var url = $"{BaseUrl}?timestamp={_store.LastTimestamp}&betTypeIds=-1&sportTypeIds={SportTypeId}&statusId=0";
            var json = await _http.GetStringAsync(url, ct);
            var root = JsonDocument.Parse(json).RootElement;
            _store.Apply(root);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to poll event updates.");
        }
    }
}