using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DziennikPodrozy.Models;

public class Trip
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }

    [Display(Name = "Opis")]
    public string? Description { get; set; }
    public int CountryId { get; set; }

    [ValidateNever]
    public Country Country { get; set; } = null!;

    public int UserId { get; set; }

    [ValidateNever]
    public UserAccount User { get; set; } = null!;

    [ValidateNever]
    public ICollection<JournalEntry> JournalEntries { get; set; } = [];

    [ValidateNever]
    public ICollection<Expense> Expenses { get; set; } = [];
}
