namespace DziennikPodrozy.Models.ViewModels;

public class DashboardViewModel
{
    public string Login { get; set; } = "";
    public bool IsAdmin { get; set; }
    public string ScopeLabel { get; set; } = "";

    public int TripCount { get; set; }
    public int VisitedCountryCount { get; set; }
    public int EntryCount { get; set; }
    public decimal ExpenseSum { get; set; }
    public double? AvgPlaceRating { get; set; }

    public List<TripListItem> RecentTrips { get; set; } = [];
    public List<EntryListItem> RecentEntries { get; set; } = [];
}

public class TripListItem
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string CountryName { get; set; } = "";
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
}

public class EntryListItem
{
    public string Title { get; set; } = "";
    public string TripTitle { get; set; } = "";
    public DateTime EntryDate { get; set; }
    public int Rating { get; set; }
}
