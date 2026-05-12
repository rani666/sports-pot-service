using Microsoft.Extensions.Options;
using MongoDB.Driver;
using sports_pot_service.Models;

namespace sports_pot_service.Services;

public class JackpotConfigService
{
    private static readonly HashSet<string> ValidTriggerEvents =
        new(StringComparer.OrdinalIgnoreCase) { "Goal", "Offside", "Corner", "Card" };

    private readonly IMongoCollection<GlobalJackpotConfig> _collection;

    public JackpotConfigService(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<GlobalJackpotConfig>("jackpot-config");
    }

    public async Task<GlobalJackpotConfig> GetAsync()
    {
        var config = await _collection.Find(_ => true).FirstOrDefaultAsync();
        if (config is not null) return config;

        var defaultConfig = new GlobalJackpotConfig
        {
            LiveEnabled = true,
            DefaultSeed = 100,
            DefaultContribution = 0.1,
            DefaultCap = 1000,
            TriggerEvents = ["Goal", "Offside", "Corner", "Card"],
            MinOdds = 1.5,
            MinStake = 1,
            EligibleMarkets = ["Match winner", "Over/under"],
            EligibleSportsLeagues = ["Soccer — EPL", "Soccer — UCL"]
        };

        await _collection.InsertOneAsync(defaultConfig);
        return defaultConfig;
    }

    public (bool valid, string? error) Validate(GlobalJackpotConfig config)
    {
        if (config.DefaultContribution is < 0 or > 1)
            return (false, "defaultContribution must be between 0 and 1.");

        if (config.DefaultCap < config.DefaultSeed)
            return (false, "defaultCap must be greater than or equal to defaultSeed.");

        if (config.DefaultSeed < 0)
            return (false, "defaultSeed must be non-negative.");

        if (config.MinOdds < 1)
            return (false, "minOdds must be >= 1.");

        if (config.MinStake < 0)
            return (false, "minStake must be non-negative.");

        var invalidTriggers = config.TriggerEvents
            .Where(t => !ValidTriggerEvents.Contains(t))
            .ToList();

        if (invalidTriggers.Count > 0)
            return (false, $"Invalid triggerEvents: {string.Join(", ", invalidTriggers)}. Allowed: {string.Join(", ", ValidTriggerEvents)}.");

        return (true, null);
    }

    public async Task<GlobalJackpotConfig> UpsertAsync(GlobalJackpotConfig config)
    {
        var existing = await _collection.Find(_ => true).FirstOrDefaultAsync();

        if (existing is null)
        {
            await _collection.InsertOneAsync(config);
            return config;
        }

        config.Id = existing.Id;
        await _collection.ReplaceOneAsync(x => x.Id == existing.Id, config);
        return config;
    }
}
