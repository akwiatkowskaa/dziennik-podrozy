using DziennikPodrozy.Data;
using DziennikPodrozy.Filters;
using DziennikPodrozy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DziennikPodrozy.Controllers.Api;

[ApiController]
[Route("api/countries")]
[ApiAuth]
public class CountriesApiController : ControllerBase
{
    private readonly AppDbContext _db;

    public CountriesApiController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Country>>> GetAll() =>
        await _db.Countries.OrderBy(c => c.Name).ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Country>> Get(int id)
    {
        var country = await _db.Countries.FindAsync(id);
        return country == null ? NotFound() : country;
    }

    [HttpPost]
    public async Task<ActionResult<Country>> Create(Country country)
    {
        _db.Countries.Add(country);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = country.Id }, country);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Country country)
    {
        if (id != country.Id) return BadRequest();
        _db.Entry(country).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var country = await _db.Countries.FindAsync(id);
        if (country == null) return NotFound();
        _db.Countries.Remove(country);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
