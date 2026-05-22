using DziennikPodrozy.Models;
using DziennikPodrozy.Services;

namespace DziennikPodrozy.Data;

public static class DbInitializer
{
    public static void Initialize(AppDbContext db)
    {
        db.Database.EnsureCreated();

        if (db.Users.Any())
            return;

        var admin = new UserAccount
        {
            Login = "admin",
            PasswordHash = PasswordHasher.Hash("123"),
            Role = "Admin",
            ApiToken = Guid.NewGuid().ToString("N")
        };
        var user = new UserAccount
        {
            Login = "test",
            PasswordHash = PasswordHasher.Hash("haslo"),
            Role = "User",
            ApiToken = Guid.NewGuid().ToString("N")
        };
        db.Users.AddRange(admin, user);

        var pl = new Country { Name = "Polska", Code = "PL" };
        var it = new Country { Name = "Włochy", Code = "IT" };
        var cz = new Country { Name = "Czechy", Code = "CZ" };
        db.Countries.AddRange(pl, it, cz);
        db.SaveChanges();

        var trip = new Trip
        {
            Title = "Weekend w Krakowie",
            DateFrom = new DateTime(2025, 3, 15),
            DateTo = new DateTime(2025, 3, 17),
            Description = "Krótki wypad ze znajomymi",
            CountryId = pl.Id,
            UserId = admin.Id
        };
        db.Trips.Add(trip);
        db.SaveChanges();

        db.JournalEntries.AddRange(
            new JournalEntry
            {
                TripId = trip.Id,
                EntryDate = new DateTime(2025, 3, 15),
                Title = "Rynek Główny",
                Content = "Spacer i obiad na rynku.",
                Rating = 5
            },
            new JournalEntry
            {
                TripId = trip.Id,
                EntryDate = new DateTime(2025, 3, 16),
                Title = "Wawel",
                Content = "Zwiedzanie zamku.",
                Rating = 4
            });

        db.Expenses.AddRange(
            new Expense
            {
                TripId = trip.Id,
                Amount = 45.50m,
                Description = "Obiad",
                ExpenseDate = new DateTime(2025, 3, 15),
                Category = "jedzenie"
            },
            new Expense
            {
                TripId = trip.Id,
                Amount = 120m,
                Description = "Pociąg",
                ExpenseDate = new DateTime(2025, 3, 15),
                Category = "transport"
            },
            new Expense
            {
                TripId = trip.Id,
                Amount = 280m,
                Description = "Hostel 2 noce",
                ExpenseDate = new DateTime(2025, 3, 15),
                Category = "nocleg"
            });

        db.SaveChanges();
    }
}
