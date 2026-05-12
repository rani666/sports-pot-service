using Microsoft.Extensions.Options;
using MongoDB.Driver;
using sports_pot_service.Models;

namespace sports_pot_service.Services;

public class SportsPotService
{
    private readonly IMongoCollection<SportsPot> _collection;

    public SportsPotService(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<SportsPot>("sports-pots");
    }

    public async Task<List<SportsPot>> GetAllAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public async Task<SportsPot?> GetByIdAsync(string id) =>
        await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<SportsPot> CreateAsync(SportsPot sportsPot)
    {
        await _collection.InsertOneAsync(sportsPot);
        return sportsPot;
    }

    public async Task UpdateAsync(string id, SportsPot sportsPot) =>
        await _collection.ReplaceOneAsync(x => x.Id == id, sportsPot);

    public async Task DeleteAsync(string id) =>
        await _collection.DeleteOneAsync(x => x.Id == id);
}
