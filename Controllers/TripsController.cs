using DziennikPodrozy.Data;
using DziennikPodrozy.Filters;
using DziennikPodrozy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DziennikPodrozy.Controllers;

[RequireLogin]
public class TripsController : Controller
{
    private readonly AppDbContext _db;

    public TripsController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var trips = await _db.Trips
            .Include(t => t.Country)
            .Include(t => t.User)
            .OrderByDescending(t => t.DateFrom)
            .ToListAsync();
        return View(trips);
    }

    public async Task<IActionResult> Create()
    {
        await FillSelectLists();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Trip trip)
    {
        if (!ModelState.IsValid)
        {
            await FillSelectLists();
            return View(trip);
        }
        _db.Trips.Add(trip);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        var trip = await _db.Trips.FindAsync(id);
        if (trip == null) return NotFound();
        await FillSelectLists();
        return View(trip);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Trip trip)
    {
        if (id != trip.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            await FillSelectLists();
            return View(trip);
        }
        _db.Update(trip);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        var trip = await _db.Trips
            .Include(t => t.Country)
            .FirstOrDefaultAsync(t => t.Id == id);
        if (trip == null) return NotFound();
        return View(trip);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var trip = await _db.Trips.FindAsync(id);
        if (trip != null)
        {
            _db.Trips.Remove(trip);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task FillSelectLists()
    {
        ViewBag.CountryId = new SelectList(await _db.Countries.OrderBy(c => c.Name).ToListAsync(), "Id", "Name");
        ViewBag.UserId = new SelectList(await _db.Users.OrderBy(u => u.Login).ToListAsync(), "Id", "Login");
    }
}
