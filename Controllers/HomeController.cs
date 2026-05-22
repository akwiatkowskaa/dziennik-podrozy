using DziennikPodrozy.Data;
using DziennikPodrozy.Filters;
using DziennikPodrozy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DziennikPodrozy.Controllers;

[RequireLogin]
public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        ViewBag.TripCount = await _db.Trips.CountAsync();
        ViewBag.CountryCount = await _db.Countries.CountAsync();
        ViewBag.EntryCount = await _db.JournalEntries.CountAsync();
        ViewBag.ExpenseSum = await _db.Expenses.SumAsync(e => (decimal?)e.Amount) ?? 0;
        ViewBag.AvgRating = await _db.JournalEntries.AverageAsync(e => (double?)e.Rating) ?? 0;
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
}
