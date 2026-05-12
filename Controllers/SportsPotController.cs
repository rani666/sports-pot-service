using Microsoft.AspNetCore.Mvc;
using sports_pot_service.Models;
using sports_pot_service.Services;

namespace sports_pot_service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SportsPotController : ControllerBase
{
    private readonly SportsPotService _service;

    public SportsPotController(SportsPotService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<SportsPot>>> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<SportsPot>> GetById(string id)
    {
        var item = await _service.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<SportsPot>> Create(SportsPot sportsPot)
    {
        var created = await _service.CreateAsync(sportsPot);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, SportsPot sportsPot)
    {
        var existing = await _service.GetByIdAsync(id);
        if (existing is null) return NotFound();

        sportsPot.Id = id;
        await _service.UpdateAsync(id, sportsPot);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existing = await _service.GetByIdAsync(id);
        if (existing is null) return NotFound();

        await _service.DeleteAsync(id);
        return NoContent();
    }
}