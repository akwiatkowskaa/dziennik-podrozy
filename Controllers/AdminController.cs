using DziennikPodrozy.Data;
using DziennikPodrozy.Filters;
using DziennikPodrozy.Models;
using DziennikPodrozy.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DziennikPodrozy.Controllers;

[RequireAdmin]
public class AdminController : Controller
{
    private readonly AppDbContext _db;

    public AdminController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Users() =>
        View(await _db.Users.OrderBy(u => u.Login).ToListAsync());

    public IActionResult CreateUser() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(string login, string haslo, string role)
    {
        login = (login ?? "").Trim();
        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(haslo))
        {
            ViewBag.Error = "Login i hasło są wymagane.";
            return View();
        }

        if (await _db.Users.AnyAsync(u => u.Login == login))
        {
            ViewBag.Error = "Taki login już istnieje.";
            return View();
        }

        role = role == "Admin" ? "Admin" : "User";
        _db.Users.Add(new UserAccount
        {
            Login = login,
            PasswordHash = PasswordHasher.Hash(haslo),
            Role = role,
            ApiToken = Guid.NewGuid().ToString("N")
        });
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Users));
    }
}
