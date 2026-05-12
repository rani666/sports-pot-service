namespace sports_pot_service.Models;

public class LiveJackpotFeedResponse
{
    public long Timestamp { get; set; }
    public List<LiveJackpotItem> Items { get; set; } = new();
}

public class LiveJackpotItem
{
    public int Id { get; set; }
    public string GameTime { get; set; } = string.Empty;
    public NamedRef Sport { get; set; } = new();
    public JackpotInfo Jackpot { get; set; } = new();
    public NamedRef Country { get; set; } = new();
    public NamedRef League { get; set; } = new();
    public NamedRef HomeTeam { get; set; } = new();
    public NamedRef AwayTeam { get; set; } = new();
}

public class JackpotInfo
{
    public double Amount { get; set; }
    public string Trigger { get; set; } = string.Empty;
    public int PlayersIn { get; set; }
}
