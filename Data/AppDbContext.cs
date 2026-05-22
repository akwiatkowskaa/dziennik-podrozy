using DziennikPodrozy.Models;
using Microsoft.EntityFrameworkCore;

namespace DziennikPodrozy.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UserAccount> Users => Set<UserAccount>();
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<Trip> Trips => Set<Trip>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<Expense> Expenses => Set<Expense>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAccount>()
            .HasIndex(u => u.Login)
            .IsUnique();

        modelBuilder.Entity<Trip>()
            .HasOne(t => t.Country)
            .WithMany(c => c.Trips)
            .HasForeignKey(t => t.CountryId);

        modelBuilder.Entity<Trip>()
            .HasOne(t => t.User)
            .WithMany(u => u.Trips)
            .HasForeignKey(t => t.UserId);

        modelBuilder.Entity<JournalEntry>()
            .HasOne(e => e.Trip)
            .WithMany(t => t.JournalEntries)
            .HasForeignKey(e => e.TripId);

        modelBuilder.Entity<Expense>()
            .HasOne(e => e.Trip)
            .WithMany(t => t.Expenses)
            .HasForeignKey(e => e.TripId);
    }
}
