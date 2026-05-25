using DziennikPodrozy.Data;
using DziennikPodrozy.Models;
using Microsoft.EntityFrameworkCore;

namespace DziennikPodrozy.Services;

public static class UserScope
{
    public static int? GetUserId(ISession session) => session.GetInt32("UserId");

    public static bool IsAdmin(ISession session) =>
        string.Equals(session.GetString("Role"), "Admin", StringComparison.OrdinalIgnoreCase);

    public static IQueryable<Trip> TripsFor(AppDbContext db, ISession session)
    {
        var trips = db.Trips.AsQueryable();
        if (!IsAdmin(session) && GetUserId(session) is int userId)
            trips = trips.Where(t => t.UserId == userId);
        return trips;
    }

    public static IQueryable<JournalEntry> EntriesFor(AppDbContext db, ISession session) =>
        db.JournalEntries.Where(e => TripsFor(db, session).Select(t => t.Id).Contains(e.TripId));

    public static IQueryable<Expense> ExpensesFor(AppDbContext db, ISession session) =>
        db.Expenses.Where(e => TripsFor(db, session).Select(t => t.Id).Contains(e.TripId));

    public static async Task<Trip?> FindTripAsync(AppDbContext db, ISession session, int id) =>
        await TripsFor(db, session).FirstOrDefaultAsync(t => t.Id == id);
}
