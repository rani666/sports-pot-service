using Microsoft.Extensions.Options;
using MongoDB.Driver;
using sports_pot_service.Models;

namespace sports_pot_service.Services;

public class JackpotPotService
{
    private readonly IMongoCollection<JackpotPot> _collection;

    public JackpotPotService(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<JackpotPot>("jackpot-pots");
    }

    public async Task<List<JackpotPot>> GetLiveAsync() =>
        await _collection.Find(p => p.IsLive).ToListAsync();

    public async Task<JackpotPot?> GetByMatchIdAsync(int matchId) =>
        await _collection.Find(p => p.MatchId == matchId).FirstOrDefaultAsync();

    /// <summary>
    /// Records a jackpot-eligible stake: grows the pot by contribution amount,
    /// caps it at defaultCap, and registers the player.
    /// </summary>
    public async Task ApplyContributionAsync(
        int matchId,
        string userId,
        double stakeContribution,
        double cap)
    {
        var pot = await GetByMatchIdAsync(matchId);
        if (pot is null) return;

        pot.Amount = Math.Min(pot.Amount + stakeContribution, cap);
        pot.ActivePlayerIds.Add(userId);

        await _collection.ReplaceOneAsync(p => p.MatchId == matchId, pot);
    }

    public async Task<JackpotPot> CreateAsync(JackpotPot pot)
    {
        await _collection.InsertOneAsync(pot);
        return pot;
    }

    public async Task SetLiveAsync(int matchId, bool isLive)
    {
        var update = Builders<JackpotPot>.Update.Set(p => p.IsLive, isLive);
        await _collection.UpdateOneAsync(p => p.MatchId == matchId, update);
    }
}
