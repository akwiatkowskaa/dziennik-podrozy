using DziennikPodrozy.Data;
using DziennikPodrozy.Filters;
using DziennikPodrozy.Models;
using DziennikPodrozy.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DziennikPodrozy.Controllers;

[RequireLogin]
public class JournalEntriesController : Controller
{
    private readonly AppDbContext _db;

    public JournalEntriesController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var entries = await UserScope.EntriesFor(_db, HttpContext.Session)
            .Include(e => e.Trip)
            .OrderByDescending(e => e.EntryDate)
            .ToListAsync();
        return View(entries);
    }

    public async Task<IActionResult> Create()
    {
        await FillTripList();
        return View(new JournalEntry { EntryDate = DateTime.Today, Rating = 3 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(JournalEntry entry)
    {
        if (!await CanUseTrip(entry.TripId))
        {
            ModelState.AddModelError(nameof(entry.TripId), "Wybierz jedną ze swoich podróży.");
        }
        if (!ModelState.IsValid)
        {
            await FillTripList(entry.TripId);
            return View(entry);
        }
        _db.JournalEntries.Add(entry);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        var entry = await UserScope.EntriesFor(_db, HttpContext.Session)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (entry == null) return NotFound();
        await FillTripList(entry.TripId);
        return View(entry);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, JournalEntry entry)
    {
        if (id != entry.Id) return NotFound();
        if (!await CanUseTrip(entry.TripId))
            ModelState.AddModelError(nameof(entry.TripId), "Wybierz jedną ze swoich podróży.");
        if (!ModelState.IsValid)
        {
            await FillTripList(entry.TripId);
            return View(entry);
        }
        _db.Update(entry);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        var entry = await UserScope.EntriesFor(_db, HttpContext.Session)
            .Include(e => e.Trip)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (entry == null) return NotFound();
        return View(entry);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var entry = await UserScope.EntriesFor(_db, HttpContext.Session)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (entry != null)
        {
            _db.JournalEntries.Remove(entry);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task FillTripList(int? selected = null)
    {
        var trips = await UserScope.TripsFor(_db, HttpContext.Session)
            .OrderByDescending(t => t.DateFrom)
            .ToListAsync();
        ViewBag.TripId = new SelectList(trips, "Id", "Title", selected);
    }

    private async Task<bool> CanUseTrip(int tripId) =>
        await UserScope.TripsFor(_db, HttpContext.Session).AnyAsync(t => t.Id == tripId);
}
