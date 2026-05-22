namespace DziennikPodrozy.Models;

public class Expense
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = "";
    public DateTime ExpenseDate { get; set; }
    public string Category { get; set; } = "";
    public int TripId { get; set; }
    public Trip Trip { get; set; } = null!;
}
