using DziennikPodrozy.Data;
using DziennikPodrozy.Filters;
using DziennikPodrozy.Models;
using DziennikPodrozy.Services;
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
        var trips = await UserScope.TripsFor(_db, HttpContext.Session)
            .Include(t => t.Country)
            .Include(t => t.User)
            .OrderByDescending(t => t.DateFrom)
            .ToListAsync();
        ViewBag.IsAdmin = UserScope.IsAdmin(HttpContext.Session);
        return View(trips);
    }

    public async Task<IActionResult> Create()
    {
        var trip = new Trip
        {
            DateFrom = DateTime.Today,
            DateTo = DateTime.Today.AddDays(1)
        };
        if (!UserScope.IsAdmin(HttpContext.Session) && UserScope.GetUserId(HttpContext.Session) is int uid)
            trip.UserId = uid;

        await FillSelectLists(trip);
        return View(trip);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Trip trip)
    {
        ApplyUserId(trip);

        if (trip.CountryId <= 0)
            ModelState.AddModelError(nameof(trip.CountryId), "Wybierz kraj.");
        if (string.IsNullOrWhiteSpace(trip.Title))
            ModelState.AddModelError(nameof(trip.Title), "Podaj tytuł podróży.");
        if (trip.UserId <= 0)
            ModelState.AddModelError("", "Sesja wygasła — zaloguj się ponownie.");

        if (!ModelState.IsValid)
        {
            await FillSelectLists(trip);
            return View(trip);
        }

        _db.Trips.Add(trip);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        var trip = await UserScope.FindTripAsync(_db, HttpContext.Session, id ?? 0);
        if (trip == null) return NotFound();
        await FillSelectLists(trip);
        return View(trip);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Trip trip)
    {
        if (id != trip.Id) return NotFound();

        var existing = await UserScope.FindTripAsync(_db, HttpContext.Session, id);
        if (existing == null) return NotFound();

        ApplyUserId(trip);

        if (trip.CountryId <= 0)
            ModelState.AddModelError(nameof(trip.CountryId), "Wybierz kraj.");
        if (string.IsNullOrWhiteSpace(trip.Title))
            ModelState.AddModelError(nameof(trip.Title), "Podaj tytuł podróży.");

        if (!ModelState.IsValid)
        {
            await FillSelectLists(trip);
            return View(trip);
        }

        existing.Title = trip.Title;
        existing.DateFrom = trip.DateFrom;
        existing.DateTo = trip.DateTo;
        existing.Description = trip.Description;
        existing.CountryId = trip.CountryId;
        existing.UserId = trip.UserId;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        var trip = await UserScope.TripsFor(_db, HttpContext.Session)
            .Include(t => t.Country)
            .FirstOrDefaultAsync(t => t.Id == id);
        if (trip == null) return NotFound();
        return View(trip);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var trip = await UserScope.FindTripAsync(_db, HttpContext.Session, id);
        if (trip != null)
        {
            _db.Trips.Remove(trip);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private void ApplyUserId(Trip trip)
    {
        if (!UserScope.IsAdmin(HttpContext.Session) && UserScope.GetUserId(HttpContext.Session) is int uid)
            trip.UserId = uid;
    }

    private async Task FillSelectLists(Trip? trip = null)
    {
        var countries = await _db.Countries.OrderBy(c => c.Name).ToListAsync();
        var countryItems = countries
            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
            .ToList();
        countryItems.Insert(0, new SelectListItem { Value = "", Text = "— wybierz kraj —" });
        ViewBag.CountryId = new SelectList(countryItems, "Value", "Text", trip?.CountryId > 0 ? trip.CountryId.ToString() : "");
        ViewBag.IsAdmin = UserScope.IsAdmin(HttpContext.Session);
        if (ViewBag.IsAdmin)
        {
            var users = await _db.Users.OrderBy(u => u.Login).ToListAsync();
            ViewBag.UserId = new SelectList(users, "Id", "Login", trip?.UserId);
        }
    }
}
