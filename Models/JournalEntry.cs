using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DziennikPodrozy.Models;

public class JournalEntry
{
    public int Id { get; set; }
    public DateTime EntryDate { get; set; }
    public string Title { get; set; } = "";

    [Display(Name = "Opis")]
    public string? Content { get; set; }
    public int Rating { get; set; }
    public int TripId { get; set; }

    [ValidateNever]
    public Trip Trip { get; set; } = null!;
}
