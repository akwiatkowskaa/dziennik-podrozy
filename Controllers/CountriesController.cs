using DziennikPodrozy.Data;
using DziennikPodrozy.Filters;
using DziennikPodrozy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DziennikPodrozy.Controllers;

[RequireLogin]
public class CountriesController : Controller
{
    private readonly AppDbContext _db;

    public CountriesController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index() =>
        View(await _db.Countries.OrderBy(c => c.Name).ToListAsync());

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
