using DziennikPodrozy.Data;
using DziennikPodrozy.Filters;
using DziennikPodrozy.Models;
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
        var entries = await _db.JournalEntries
            .Include(e => e.Trip)
            .OrderByDescending(e => e.EntryDate)
            .ToListAsync();
        return View(entries);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.TripId = new SelectList(
            await _db.Trips.OrderByDescending(t => t.DateFrom).ToListAsync(), "Id", "Title");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(JournalEntry entry)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.TripId = new SelectList(
                await _db.Trips.ToListAsync(), "Id", "Title", entry.TripId);
            return View(entry);
        }
        _db.JournalEntries.Add(entry);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        var entry = await _db.JournalEntries.FindAsync(id);
        if (entry == null) return NotFound();
        ViewBag.TripId = new SelectList(await _db.Trips.ToListAsync(), "Id", "Title", entry.TripId);
        return View(entry);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, JournalEntry entry)
    {
        if (id != entry.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            ViewBag.TripId = new SelectList(await _db.Trips.ToListAsync(), "Id", "Title", entry.TripId);
            return View(entry);
        }
        _db.Update(entry);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        var entry = await _db.JournalEntries
            .Include(e => e.Trip)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (entry == null) return NotFound();
        return View(entry);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var entry = await _db.JournalEntries.FindAsync(id);
        if (entry != null)
        {
            _db.JournalEntries.Remove(entry);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
