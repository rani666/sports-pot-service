using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace sports_pot_service.Models;

/// <summary>Per-match jackpot state persisted in MongoDB.</summary>
public class JackpotPot
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public int MatchId { get; set; }
    public double Amount { get; set; }
    public string Trigger { get; set; } = string.Empty;

    /// <summary>Distinct user IDs with an active eligible selection on this match.</summary>
    public HashSet<string> ActivePlayerIds { get; set; } = new();

    public string SportLeagueKey { get; set; } = string.Empty;
    public string GameTime { get; set; } = string.Empty;
    public NamedRef Sport { get; set; } = new();
    public NamedRef Country { get; set; } = new();
    public NamedRef League { get; set; } = new();
    public NamedRef HomeTeam { get; set; } = new();
    public NamedRef AwayTeam { get; set; } = new();
    public bool IsLive { get; set; } = true;
}

public class NamedRef
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
