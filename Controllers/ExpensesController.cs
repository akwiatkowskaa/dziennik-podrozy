using DziennikPodrozy.Data;
using DziennikPodrozy.Filters;
using DziennikPodrozy.Models;
using DziennikPodrozy.Services;
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
        var expenses = await UserScope.ExpensesFor(_db, HttpContext.Session)
            .Include(e => e.Trip)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync();
        return View(expenses);
    }

    public async Task<IActionResult> Create()
    {
        await FillTripList();
        return View(new Expense { ExpenseDate = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Expense expense)
    {
        if (!await CanUseTrip(expense.TripId))
            ModelState.AddModelError(nameof(expense.TripId), "Wybierz jedną ze swoich podróży.");
        if (!ModelState.IsValid)
        {
            await FillTripList(expense.TripId);
            return View(expense);
        }
        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        var expense = await UserScope.ExpensesFor(_db, HttpContext.Session)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (expense == null) return NotFound();
        await FillTripList(expense.TripId);
        return View(expense);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Expense expense)
    {
        if (id != expense.Id) return NotFound();
        if (!await CanUseTrip(expense.TripId))
            ModelState.AddModelError(nameof(expense.TripId), "Wybierz jedną ze swoich podróży.");
        if (!ModelState.IsValid)
        {
            await FillTripList(expense.TripId);
            return View(expense);
        }
        _db.Update(expense);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        var expense = await UserScope.ExpensesFor(_db, HttpContext.Session)
            .Include(e => e.Trip)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (expense == null) return NotFound();
        return View(expense);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var expense = await UserScope.ExpensesFor(_db, HttpContext.Session)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (expense != null)
        {
            _db.Expenses.Remove(expense);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task FillTripList(int? selected = null)
    {
        var trips = await UserScope.TripsFor(_db, HttpContext.Session)
            .OrderByDescending(t => t.DateFrom)
            .ToListAsync();
        ViewBag.TripId = new SelectList(trips, "Id", "Title", selected);
    }

    private async Task<bool> CanUseTrip(int tripId) =>
        await UserScope.TripsFor(_db, HttpContext.Session).AnyAsync(t => t.Id == tripId);
}
