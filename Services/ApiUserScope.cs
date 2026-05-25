using DziennikPodrozy.Data;
using DziennikPodrozy.Models;
using Microsoft.EntityFrameworkCore;

namespace DziennikPodrozy.Services;

public static class ApiUserScope
{
    public static UserAccount? GetUser(HttpContext ctx) =>
        ctx.Items["ApiUser"] as UserAccount;

    public static bool IsAdmin(UserAccount user) =>
        string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase);

    public static IQueryable<Trip> TripsFor(AppDbContext db, UserAccount user)
    {
        var trips = db.Trips.AsQueryable();
        if (!IsAdmin(user))
            trips = trips.Where(t => t.UserId == user.Id);
        return trips;
    }

    public static IQueryable<JournalEntry> EntriesFor(AppDbContext db, UserAccount user) =>
        db.JournalEntries.Where(e => TripsFor(db, user).Select(t => t.Id).Contains(e.TripId));

    public static IQueryable<Expense> ExpensesFor(AppDbContext db, UserAccount user) =>
        db.Expenses.Where(e => TripsFor(db, user).Select(t => t.Id).Contains(e.TripId));

    public static Task<Trip?> FindTripAsync(AppDbContext db, UserAccount user, int id) =>
        TripsFor(db, user).FirstOrDefaultAsync(t => t.Id == id);

    public static Task<bool> OwnsTripAsync(AppDbContext db, UserAccount user, int tripId) =>
        TripsFor(db, user).AnyAsync(t => t.Id == tripId);
}
