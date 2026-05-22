namespace DziennikPodrozy.Models;

public class Trip
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public string? Description { get; set; }
    public int CountryId { get; set; }
    public Country Country { get; set; } = null!;
    public int UserId { get; set; }
    public UserAccount User { get; set; } = null!;
    public ICollection<JournalEntry> JournalEntries { get; set; } = [];
    public ICollection<Expense> Expenses { get; set; } = [];
}
