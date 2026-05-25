using DziennikPodrozy.Data;
using DziennikPodrozy.Filters;
using DziennikPodrozy.Models;
using DziennikPodrozy.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DziennikPodrozy.Controllers.Api;

[ApiController]
[Route("api/trips")]
[ApiAuth]
public class TripsApiController : ControllerBase
{
    private readonly AppDbContext _db;

    public TripsApiController(AppDbContext db) => _db = db;

    private UserAccount Current => ApiUserScope.GetUser(HttpContext)!;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAll()
    {
        var list = await ApiUserScope.TripsFor(_db, Current)
            .Include(t => t.Country)
            .OrderByDescending(t => t.DateFrom)
            .Select(t => new
            {
                t.Id,
                t.Title,
                t.DateFrom,
                t.DateTo,
                t.Description,
                t.CountryId,
                CountryName = t.Country.Name,
                t.UserId
            })
            .ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Trip>> Get(int id)
    {
        var trip = await ApiUserScope.FindTripAsync(_db, Current, id);
        return trip == null ? NotFound() : trip;
    }

    [HttpPost]
    public async Task<ActionResult<Trip>> Create(Trip trip)
    {
        if (!ApiUserScope.IsAdmin(Current))
            trip.UserId = Current.Id;

        _db.Trips.Add(trip);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = trip.Id }, trip);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Trip trip)
    {
        if (id != trip.Id) return BadRequest();
        if (await ApiUserScope.FindTripAsync(_db, Current, id) == null)
            return NotFound();
        if (!ApiUserScope.IsAdmin(Current))
            trip.UserId = Current.Id;

        _db.Entry(trip).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var trip = await ApiUserScope.FindTripAsync(_db, Current, id);
        if (trip == null) return NotFound();
        _db.Trips.Remove(trip);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
