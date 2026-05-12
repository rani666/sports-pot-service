using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace sports_pot_service.Models;

public class GlobalJackpotConfig
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public bool LiveEnabled { get; set; } = true;
    public double DefaultSeed { get; set; } = 100;

    /// <summary>Share of each eligible stake added to the pot (0–1).</summary>
    public double DefaultContribution { get; set; } = 0.1;

    public double DefaultCap { get; set; } = 1000;

    /// <summary>Allowed values: Goal, Offside, Corner, Card.</summary>
    public List<string> TriggerEvents { get; set; } = new();

    public double MinOdds { get; set; } = 1.5;
    public double MinStake { get; set; } = 1;

    public List<string> EligibleMarkets { get; set; } = new();
    public List<string> EligibleSportsLeagues { get; set; } = new();
}
