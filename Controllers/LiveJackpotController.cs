using Microsoft.AspNetCore.Mvc;
using sports_pot_service.Models;
using sports_pot_service.Services;

namespace sports_pot_service.Controllers;

[ApiController]
public class LiveJackpotController : ControllerBase
{
    private readonly LiveJackpotService _service;

    public LiveJackpotController(LiveJackpotService service)
    {
        _service = service;
    }

    /// <summary>Public live jackpot feed. Returns items: [] when liveEnabled is false or no matches qualify.</summary>
    [HttpGet("/live-jackpot")]
    public async Task<ActionResult<LiveJackpotFeedResponse>> GetFeed() =>
        Ok(await _service.GetFeedAsync());

    /// <summary>
    /// Evaluates and tags a selection. Returns the enriched selection.
    /// sourceModule will be 'jackpot_event' when all eligibility criteria are met.
    /// </summary>
    [HttpPost("/live-jackpot/tag-selection")]
    public async Task<ActionResult<Selection>> TagSelection(Selection selection)
    {
        await _service.TagSelectionAsync(selection);
        return Ok(selection);
    }
}