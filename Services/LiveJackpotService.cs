using sports_pot_service.Models;

namespace sports_pot_service.Services;

public class LiveJackpotService
{
    private readonly JackpotConfigService _configService;
    private readonly JackpotPotService _potService;

    public LiveJackpotService(JackpotConfigService configService, JackpotPotService potService)
    {
        _configService = configService;
        _potService = potService;
    }

    public async Task<LiveJackpotFeedResponse> GetFeedAsync()
    {
        var config = await _configService.GetAsync();

        if (!config.LiveEnabled)
            return EmptyFeed();

        var pots = await _potService.GetLiveAsync();

        var eligibleKeys = new HashSet<string>(config.EligibleSportsLeagues, StringComparer.OrdinalIgnoreCase);

        var items = pots
            .Where(p => eligibleKeys.Contains(p.SportLeagueKey))
            .Select(p => new LiveJackpotItem
            {
                Id = p.MatchId,
                GameTime = p.GameTime,
                Sport = p.Sport,
                Country = p.Country,
                League = p.League,
                HomeTeam = p.HomeTeam,
                AwayTeam = p.AwayTeam,
                Jackpot = new JackpotInfo
                {
                    Amount = p.Amount,
                    Trigger = p.Trigger,
                    PlayersIn = p.ActivePlayerIds.Count
                }
            })
            .ToList();

        return new LiveJackpotFeedResponse
        {
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Items = items
        };
    }

    /// <summary>
    /// Evaluates whether a selection qualifies for jackpot tagging and enriches it.
    /// Returns true when sourceModule is set to 'jackpot_event'.
    /// </summary>
    public async Task<bool> TagSelectionAsync(Selection selection)
    {
        var config = await _configService.GetAsync();

        if (!config.LiveEnabled)
            return false;

        if (selection.Odds < config.MinOdds || selection.Stake < config.MinStake)
            return false;

        var marketEligible = config.EligibleMarkets
            .Any(m => string.Equals(m, selection.Market, StringComparison.OrdinalIgnoreCase));

        if (!marketEligible)
            return false;

        var pot = await _potService.GetByMatchIdAsync(selection.MatchId);
        if (pot is null || !pot.IsLive)
            return false;

        var leagueEligible = config.EligibleSportsLeagues
            .Any(l => string.Equals(l, pot.SportLeagueKey, StringComparison.OrdinalIgnoreCase));

        if (!leagueEligible)
            return false;

        selection.SourceModule = "jackpot_event";
        selection.JackpotContribution = Math.Round(selection.Stake * config.DefaultContribution, 2);

        await _potService.ApplyContributionAsync(
            selection.MatchId,
            selection.UserId,
            selection.JackpotContribution.Value,
            config.DefaultCap);

        return true;
    }

    private static LiveJackpotFeedResponse EmptyFeed() =>
        new() { Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), Items = [] };
}