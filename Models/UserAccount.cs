namespace DziennikPodrozy.Models;

public class UserAccount
{
    public int Id { get; set; }
    public string Login { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = "User";
    public string ApiToken { get; set; } = "";
    public ICollection<Trip> Trips { get; set; } = [];
}
