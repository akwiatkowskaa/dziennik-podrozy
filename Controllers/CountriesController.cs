using DziennikPodrozy.Data;
using DziennikPodrozy.Filters;
using DziennikPodrozy.Models;
using DziennikPodrozy.Models.ViewModels;
using DziennikPodrozy.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DziennikPodrozy.Controllers;

[RequireLogin]
public class CountriesController : Controller
{
    private readonly AppDbContext _db;

    public CountriesController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var myTripCounts = await UserScope.TripsFor(_db, HttpContext.Session)
            .GroupBy(t => t.CountryId)
            .Select(g => new { CountryId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.CountryId, x => x.Count);

        var countries = await _db.Countries.OrderBy(c => c.Name).ToListAsync();
        var rows = countries.Select(c => new CountryListRow
        {
            Id = c.Id,
            Name = c.Name,
            Code = c.Code,
            MyTripCount = myTripCounts.GetValueOrDefault(c.Id, 0)
        }).ToList();

        ViewBag.Hint = UserScope.IsAdmin(HttpContext.Session)
            ? "Słownik krajów. Kolumna „Twoje podróże” = ile podróży w tym kraju (wszyscy użytkownicy)."
            : "Słownik krajów. „Twoje podróże” = ile Twoich wyjazdów było do tego kraju (0 = kraj jest w bazie, ale tam nie byłaś).";

        return View(rows);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Country country)
    {
        if (!ModelState.IsValid) return View(country);
        _db.Countries.Add(country);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        var country = await _db.Countries.FindAsync(id);
        if (country == null) return NotFound();
        return View(country);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Country country)
    {
        if (id != country.Id) return NotFound();
        if (!ModelState.IsValid) return View(country);
        _db.Update(country);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        var country = await _db.Countries.FindAsync(id);
        if (country == null) return NotFound();
        ViewBag.TripCount = await _db.Trips.CountAsync(t => t.CountryId == id);
        return View(country);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var country = await _db.Countries.FindAsync(id);
        if (country != null)
        {
            _db.Countries.Remove(country);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}

public class CountryListRow
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Code { get; set; }
    public int MyTripCount { get; set; }
}
