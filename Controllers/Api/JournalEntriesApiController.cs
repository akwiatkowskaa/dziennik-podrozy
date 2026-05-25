using DziennikPodrozy.Data;
using DziennikPodrozy.Filters;
using DziennikPodrozy.Models;
using DziennikPodrozy.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DziennikPodrozy.Controllers.Api;

[ApiController]
[Route("api/journalentries")]
[ApiAuth]
public class JournalEntriesApiController : ControllerBase
{
    private readonly AppDbContext _db;

    public JournalEntriesApiController(AppDbContext db) => _db = db;

    private UserAccount Current => ApiUserScope.GetUser(HttpContext)!;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JournalEntry>>> GetAll() =>
        await ApiUserScope.EntriesFor(_db, Current)
            .OrderByDescending(e => e.EntryDate)
            .ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<JournalEntry>> Get(int id)
    {
        var entry = await ApiUserScope.EntriesFor(_db, Current)
            .FirstOrDefaultAsync(e => e.Id == id);
        return entry == null ? NotFound() : entry;
    }

    [HttpPost]
    public async Task<ActionResult<JournalEntry>> Create(JournalEntry entry)
    {
        if (!await ApiUserScope.OwnsTripAsync(_db, Current, entry.TripId))
            return BadRequest(new { error = "Brak dostępu do tej podróży." });

        _db.JournalEntries.Add(entry);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = entry.Id }, entry);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, JournalEntry entry)
    {
        if (id != entry.Id) return BadRequest();
        var existing = await ApiUserScope.EntriesFor(_db, Current)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (existing == null) return NotFound();
        if (!await ApiUserScope.OwnsTripAsync(_db, Current, entry.TripId))
            return BadRequest(new { error = "Brak dostępu do tej podróży." });

        _db.Entry(entry).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entry = await ApiUserScope.EntriesFor(_db, Current)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (entry == null) return NotFound();
        _db.JournalEntries.Remove(entry);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
