using DziennikPodrozy.Data;
using DziennikPodrozy.Filters;
using DziennikPodrozy.Models;
using DziennikPodrozy.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DziennikPodrozy.Controllers.Api;

[ApiController]
[Route("api/expenses")]
[ApiAuth]
public class ExpensesApiController : ControllerBase
{
    private readonly AppDbContext _db;

    public ExpensesApiController(AppDbContext db) => _db = db;

    private UserAccount Current => ApiUserScope.GetUser(HttpContext)!;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Expense>>> GetAll() =>
        await ApiUserScope.ExpensesFor(_db, Current)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Expense>> Get(int id)
    {
        var expense = await ApiUserScope.ExpensesFor(_db, Current)
            .FirstOrDefaultAsync(e => e.Id == id);
        return expense == null ? NotFound() : expense;
    }

    [HttpPost]
    public async Task<ActionResult<Expense>> Create(Expense expense)
    {
        if (!await ApiUserScope.OwnsTripAsync(_db, Current, expense.TripId))
            return BadRequest(new { error = "Brak dostępu do tej podróży." });

        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = expense.Id }, expense);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Expense expense)
    {
        if (id != expense.Id) return BadRequest();
        var existing = await ApiUserScope.ExpensesFor(_db, Current)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (existing == null) return NotFound();
        if (!await ApiUserScope.OwnsTripAsync(_db, Current, expense.TripId))
            return BadRequest(new { error = "Brak dostępu do tej podróży." });

        _db.Entry(expense).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var expense = await ApiUserScope.ExpensesFor(_db, Current)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (expense == null) return NotFound();
        _db.Expenses.Remove(expense);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
