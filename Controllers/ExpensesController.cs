using DziennikPodrozy.Data;
using DziennikPodrozy.Filters;
using DziennikPodrozy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DziennikPodrozy.Controllers;

[RequireLogin]
public class ExpensesController : Controller
{
    private readonly AppDbContext _db;

    public ExpensesController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var expenses = await _db.Expenses
            .Include(e => e.Trip)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync();
        return View(expenses);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.TripId = new SelectList(
            await _db.Trips.OrderByDescending(t => t.DateFrom).ToListAsync(), "Id", "Title");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Expense expense)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.TripId = new SelectList(await _db.Trips.ToListAsync(), "Id", "Title", expense.TripId);
            return View(expense);
        }
        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        var expense = await _db.Expenses.FindAsync(id);
        if (expense == null) return NotFound();
        ViewBag.TripId = new SelectList(await _db.Trips.ToListAsync(), "Id", "Title", expense.TripId);
        return View(expense);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Expense expense)
    {
        if (id != expense.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            ViewBag.TripId = new SelectList(await _db.Trips.ToListAsync(), "Id", "Title", expense.TripId);
            return View(expense);
        }
        _db.Update(expense);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        var expense = await _db.Expenses
            .Include(e => e.Trip)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (expense == null) return NotFound();
        return View(expense);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var expense = await _db.Expenses.FindAsync(id);
        if (expense != null)
        {
            _db.Expenses.Remove(expense);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
