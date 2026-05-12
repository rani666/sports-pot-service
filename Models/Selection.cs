namespace sports_pot_service.Models;

public class Selection
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public int MatchId { get; set; }
    public string Market { get; set; } = string.Empty;
    public double Odds { get; set; }
    public double Stake { get; set; }

    /// <summary>Set to 'jackpot_event' when this selection meets all jackpot eligibility criteria.</summary>
    public string? SourceModule { get; set; }

    public double? JackpotContribution { get; set; }
}
