using DziennikPodrozy.Data;
using DziennikPodrozy.Filters;
using DziennikPodrozy.Models.ViewModels;
using DziennikPodrozy.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DziennikPodrozy.Controllers;

[RequireLogin]
public class ReportsController : Controller
{
    private readonly AppDbContext _db;

    public ReportsController(AppDbContext db) => _db = db;

    public async Task<IActionResult> RankingKrajow()
    {
        var trips = await UserScope.TripsFor(_db, HttpContext.Session)
            .Include(t => t.Country)
            .Include(t => t.Expenses)
            .Include(t => t.JournalEntries)
            .ToListAsync();

        var rows = trips
            .GroupBy(t => new { t.CountryId, t.Country.Name, t.Country.Code })
            .Select(g => new CountryRankingRow
            {
                CountryName = g.Key.Name,
                CountryCode = g.Key.Code,
                TripCount = g.Count(),
                ExpenseSum = g.SelectMany(t => t.Expenses).Sum(e => e.Amount),
                AvgPlaceRating = g.SelectMany(t => t.JournalEntries).Any()
                    ? g.SelectMany(t => t.JournalEntries).Average(e => e.Rating)
                    : null
            })
            .OrderByDescending(r => r.TripCount)
            .ThenByDescending(r => r.ExpenseSum)
            .ToList();

        ViewBag.ScopeLabel = UserScope.IsAdmin(HttpContext.Session)
            ? "Kraje z podróży wszystkich użytkowników"
            : "Kraje z Twoich podróży (tylko tam, gdzie faktycznie byłaś)";

        return View(rows);
    }

    public async Task<IActionResult> RaportWydatkow(int? tripId)
    {
        var session = HttpContext.Session;
        var myTrips = await UserScope.TripsFor(_db, session)
            .OrderByDescending(t => t.DateFrom)
            .Select(t => new TripSelectItem { Id = t.Id, Title = t.Title })
            .ToListAsync();

        var vm = new ExpenseReportViewModel { Trips = myTrips };

        if (tripId == null && myTrips.Count > 0)
            tripId = myTrips[0].Id;

        if (tripId == null)
            return View(vm);

        var trip = await UserScope.FindTripAsync(_db, session, tripId.Value);
        if (trip == null)
            return View(vm);

        var expenses = await _db.Expenses
            .Where(e => e.TripId == tripId.Value)
            .ToListAsync();

        vm.SelectedTripId = tripId;
        vm.SelectedTripTitle = trip.Title;
        vm.Total = expenses.Sum(e => e.Amount);

        if (vm.Total > 0)
        {
            vm.Categories = expenses
                .GroupBy(e => string.IsNullOrWhiteSpace(e.Category) ? "(brak)" : e.Category)
                .Select(g => new ExpenseCategoryRow
                {
                    Category = g.Key,
                    Sum = g.Sum(e => e.Amount),
                    Percent = Math.Round(g.Sum(e => e.Amount) / vm.Total * 100, 1)
                })
                .OrderByDescending(c => c.Sum)
                .ToList();
        }

        return View(vm);
    }

    public async Task<IActionResult> PodrozeWgDat()
    {
        var today = DateTime.Today;
        var trips = await UserScope.TripsFor(_db, HttpContext.Session)
            .Include(t => t.Country)
            .OrderBy(t => t.DateFrom)
            .ToListAsync();

        var vm = new TripsByDateViewModel
        {
            Upcoming = trips.Where(t => t.DateFrom > today).Select(ToItem).ToList(),
            Ongoing = trips.Where(t => t.DateFrom <= today && t.DateTo >= today).Select(ToItem).ToList(),
            Finished = trips.Where(t => t.DateTo < today).OrderByDescending(t => t.DateTo).Select(ToItem).ToList()
        };

        ViewBag.ScopeLabel = UserScope.IsAdmin(HttpContext.Session)
            ? "Wszystkie podróże w systemie"
            : "Tylko Twoje podróże";

        return View(vm);
    }

    private static TripListItem ToItem(Models.Trip t) => new()
    {
        Id = t.Id,
        Title = t.Title,
        CountryName = t.Country.Name,
        DateFrom = t.DateFrom,
        DateTo = t.DateTo
    };
}
