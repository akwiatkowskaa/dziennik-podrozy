using DziennikPodrozy.Data;
using DziennikPodrozy.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DziennikPodrozy.Controllers;

public class AuthController : Controller
{
    private readonly AppDbContext _db;

    public AuthController(AppDbContext db) => _db = db;

    [HttpGet]
    public IActionResult Logowanie() => View();

    [HttpPost]
    public async Task<IActionResult> Logowanie(string login, string haslo)
    {
        var hash = PasswordHasher.Hash(haslo ?? "");
        var user = await _db.Users.FirstOrDefaultAsync(u =>
            u.Login == (login ?? "") && u.PasswordHash == hash);

        if (user == null)
        {
            ViewBag.Error = "Błędny login lub hasło";
            return View();
        }

        HttpContext.Session.SetString("User", user.Login);
        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("Role", user.Role);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Wyloguj()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Logowanie");
    }
}
