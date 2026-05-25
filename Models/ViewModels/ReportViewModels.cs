namespace DziennikPodrozy.Models.ViewModels;

public class CountryRankingRow
{
    public string CountryName { get; set; } = "";
    public string? CountryCode { get; set; }
    public int TripCount { get; set; }
    public decimal ExpenseSum { get; set; }
    public double? AvgPlaceRating { get; set; }
}

public class ExpenseCategoryRow
{
    public string Category { get; set; } = "";
    public decimal Sum { get; set; }
    public decimal Percent { get; set; }
}

public class ExpenseReportViewModel
{
    public int? SelectedTripId { get; set; }
    public string? SelectedTripTitle { get; set; }
    public decimal Total { get; set; }
    public List<ExpenseCategoryRow> Categories { get; set; } = [];
    public List<TripSelectItem> Trips { get; set; } = [];
}

public class TripSelectItem
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
}

public class TripsByDateViewModel
{
    public List<TripListItem> Upcoming { get; set; } = [];
    public List<TripListItem> Ongoing { get; set; } = [];
    public List<TripListItem> Finished { get; set; } = [];
}
