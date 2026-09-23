using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Repair_Service_Center.Data;
using Repair_Service_Center.Models;

namespace Repair_Service_Center.Controllers;

public class ComplexityLevelsController : Controller
{
    private readonly RepairServiceContext _context;

    public ComplexityLevelsController(RepairServiceContext context)
    {
        _context = context;
    }

    // GET: ComplexityLevels
    public async Task<IActionResult> Index()
    {
        var items = await _context.ComplexityLevels
            .OrderBy(x => x.Name)
            .ToListAsync();
        return View(items);
    }

    // GET: ComplexityLevels/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var complexityLevel = await _context.ComplexityLevels
            .FirstOrDefaultAsync(m => m.Id == id);
        if (complexityLevel == null)
        {
            return NotFound();
        }

        return View(complexityLevel);
    }

    // GET: ComplexityLevels/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: ComplexityLevels/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name")] ComplexityLevel complexityLevel)
    {
        if (ModelState.IsValid)
        {
            _context.Add(complexityLevel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(complexityLevel);
    }

    // GET: ComplexityLevels/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var complexityLevel = await _context.ComplexityLevels.FindAsync(id);
        if (complexityLevel == null)
        {
            return NotFound();
        }
        return View(complexityLevel);
    }

    // POST: ComplexityLevels/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] ComplexityLevel complexityLevel)
    {
        if (id != complexityLevel.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(complexityLevel);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComplexityLevelExists(complexityLevel.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(complexityLevel);
    }

    // GET: ComplexityLevels/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var complexityLevel = await _context.ComplexityLevels
            .FirstOrDefaultAsync(m => m.Id == id);
        if (complexityLevel == null)
        {
            return NotFound();
        }

        return View(complexityLevel);
    }

    // POST: ComplexityLevels/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var complexityLevel = await _context.ComplexityLevels
            .FirstOrDefaultAsync(m => m.Id == id);
        if (complexityLevel == null)
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            _context.ComplexityLevels.Remove(complexityLevel);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ViewData["Error"] = "This complexity level is used in the price list and cannot be deleted.";
            return View(complexityLevel);
        }

        return RedirectToAction(nameof(Index));
    }

    private bool ComplexityLevelExists(int id)
    {
        return _context.ComplexityLevels.Any(e => e.Id == id);
    }
}
