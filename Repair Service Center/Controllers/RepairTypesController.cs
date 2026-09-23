using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Repair_Service_Center.Data;
using Repair_Service_Center.Models;

namespace Repair_Service_Center.Controllers;

public class RepairTypesController : Controller
{
    private readonly RepairServiceContext _context;

    public RepairTypesController(RepairServiceContext context)
    {
        _context = context;
    }

    // GET: RepairTypes
    public async Task<IActionResult> Index()
    {
        var items = await _context.RepairTypes
            .OrderBy(x => x.Name)
            .ToListAsync();
        return View(items);
    }

    // GET: RepairTypes/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var repairType = await _context.RepairTypes
            .FirstOrDefaultAsync(m => m.Id == id);
        if (repairType == null)
        {
            return NotFound();
        }

        return View(repairType);
    }

    // GET: RepairTypes/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: RepairTypes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Description")] RepairType repairType)
    {
        if (ModelState.IsValid)
        {
            _context.Add(repairType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(repairType);
    }

    // GET: RepairTypes/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var repairType = await _context.RepairTypes.FindAsync(id);
        if (repairType == null)
        {
            return NotFound();
        }
        return View(repairType);
    }

    // POST: RepairTypes/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] RepairType repairType)
    {
        if (id != repairType.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(repairType);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RepairTypeExists(repairType.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(repairType);
    }

    // GET: RepairTypes/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var repairType = await _context.RepairTypes
            .FirstOrDefaultAsync(m => m.Id == id);
        if (repairType == null)
        {
            return NotFound();
        }

        return View(repairType);
    }

    // POST: RepairTypes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var repairType = await _context.RepairTypes
            .FirstOrDefaultAsync(m => m.Id == id);
        if (repairType == null)
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            _context.RepairTypes.Remove(repairType);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ViewData["Error"] = "This repair type is used in the price list and cannot be deleted.";
            return View(repairType);
        }

        return RedirectToAction(nameof(Index));
    }

    private bool RepairTypeExists(int id)
    {
        return _context.RepairTypes.Any(e => e.Id == id);
    }
}
