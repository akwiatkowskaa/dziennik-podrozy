namespace DziennikPodrozy.Models;

public class JournalEntry
{
    public int Id { get; set; }
    public DateTime EntryDate { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public int Rating { get; set; }
    public int TripId { get; set; }
    public Trip Trip { get; set; } = null!;
}
