using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CacauShowApi.Data;
using CacauShowApi.Models;

namespace CacauShowApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FranquiasController : ControllerBase
{
    private readonly AppDbContext _db;
    public FranquiasController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.Franquias.ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var franquia = await _db.Franquias.FindAsync(id);
        return franquia is null ? NotFound() : Ok(franquia);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Franquia franquia)
    {
        _db.Franquias.Add(franquia);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = franquia.Id }, franquia);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Franquia franquia)
    {
        if (id != franquia.Id) return BadRequest();
        _db.Entry(franquia).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var franquia = await _db.Franquias.FindAsync(id);
        if (franquia is null) return NotFound();
        _db.Franquias.Remove(franquia);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
