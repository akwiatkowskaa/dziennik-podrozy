using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DziennikPodrozy.Models;

public class Expense
{
    public int Id { get; set; }
    public decimal Amount { get; set; }

    [Display(Name = "Opis")]
    public string? Description { get; set; }
    public DateTime ExpenseDate { get; set; }
    public string Category { get; set; } = "";
    public int TripId { get; set; }

    [ValidateNever]
    public Trip Trip { get; set; } = null!;
}
