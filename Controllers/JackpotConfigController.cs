using Microsoft.AspNetCore.Mvc;
using sports_pot_service.Models;
using sports_pot_service.Services;

namespace sports_pot_service.Controllers;

[ApiController]
[Route("api/backoffice/jackpot-config")]
public class JackpotConfigController : ControllerBase
{
    private readonly JackpotConfigService _service;

    public JackpotConfigController(JackpotConfigService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<GlobalJackpotConfig>> Get() =>
        Ok(await _service.GetAsync());

    [HttpPut]
    public async Task<ActionResult<GlobalJackpotConfig>> Upsert(GlobalJackpotConfig config)
    {
        var (valid, error) = _service.Validate(config);
        if (!valid)
            return BadRequest(new { error });

        var saved = await _service.UpsertAsync(config);
        return Ok(saved);
    }
}
