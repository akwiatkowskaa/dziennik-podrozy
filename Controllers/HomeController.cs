using DziennikPodrozy.Data;
using DziennikPodrozy.Filters;
using DziennikPodrozy.Models;
using DziennikPodrozy.Models.ViewModels;
using DziennikPodrozy.Services;
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
        var session = HttpContext.Session;
        var trips = UserScope.TripsFor(_db, session);
        var entries = UserScope.EntriesFor(_db, session);
        var expenses = UserScope.ExpensesFor(_db, session);

        var vm = new DashboardViewModel
        {
            Login = session.GetString("User") ?? "",
            IsAdmin = UserScope.IsAdmin(session),
            ScopeLabel = UserScope.IsAdmin(session)
                ? "Podsumowanie wszystkich użytkowników w systemie"
                : "Podsumowanie tylko Twoich podróży",
            TripCount = await trips.CountAsync(),
            VisitedCountryCount = await trips.Select(t => t.CountryId).Distinct().CountAsync(),
            EntryCount = await entries.CountAsync(),
            ExpenseSum = await expenses.SumAsync(e => (decimal?)e.Amount) ?? 0,
            AvgPlaceRating = await entries.AnyAsync()
                ? await entries.AverageAsync(e => (double?)e.Rating)
                : null,
            RecentTrips = await trips
                .Include(t => t.Country)
                .OrderByDescending(t => t.DateFrom)
                .Take(3)
                .Select(t => new TripListItem
                {
                    Id = t.Id,
                    Title = t.Title,
                    CountryName = t.Country.Name,
                    DateFrom = t.DateFrom,
                    DateTo = t.DateTo
                })
                .ToListAsync(),
            RecentEntries = await entries
                .Include(e => e.Trip)
                .OrderByDescending(e => e.EntryDate)
                .Take(3)
                .Select(e => new EntryListItem
                {
                    Title = e.Title,
                    TripTitle = e.Trip.Title,
                    EntryDate = e.EntryDate,
                    Rating = e.Rating
                })
                .ToListAsync()
        };

        return View(vm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
}
