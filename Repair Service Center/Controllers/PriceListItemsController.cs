using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Repair_Service_Center.Data;
using Repair_Service_Center.Models;

namespace Repair_Service_Center.Controllers;

public class PriceListItemsController : Controller
{
    private readonly RepairServiceContext _context;

    public PriceListItemsController(RepairServiceContext context)
    {
        _context = context;
    }

    // GET: PriceListItems
    public async Task<IActionResult> Index()
    {
        var items = await _context.PriceListItems
            .Include(x => x.DeviceType)
            .Include(x => x.RepairType)
            .Include(x => x.ComplexityLevel)
            .OrderBy(x => x.DeviceType!.Name).ThenBy(x => x.RepairType!.Name)
            .ToListAsync();
        return View(items);
    }

    // GET: PriceListItems/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var priceListItem = await _context.PriceListItems
            .Include(x => x.DeviceType)
            .Include(x => x.RepairType)
            .Include(x => x.ComplexityLevel)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (priceListItem == null)
        {
            return NotFound();
        }

        return View(priceListItem);
    }

    // GET: PriceListItems/Create
    public IActionResult Create()
    {
        ViewData["DeviceTypeId"] = new SelectList(_context.DeviceTypes.OrderBy(x => x.Name), "Id", "Name");
        ViewData["RepairTypeId"] = new SelectList(_context.RepairTypes.OrderBy(x => x.Name), "Id", "Name");
        ViewData["ComplexityLevelId"] = new SelectList(_context.ComplexityLevels.OrderBy(x => x.Name), "Id", "Name");
        return View();
    }

    // POST: PriceListItems/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,DeviceTypeId,RepairTypeId,ComplexityLevelId,Price,DurationMinutes")] PriceListItem priceListItem)
    {
        if (ModelState.IsValid)
        {
            _context.Add(priceListItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["DeviceTypeId"] = new SelectList(_context.DeviceTypes.OrderBy(x => x.Name), "Id", "Name", priceListItem?.DeviceTypeId);
        ViewData["RepairTypeId"] = new SelectList(_context.RepairTypes.OrderBy(x => x.Name), "Id", "Name", priceListItem?.RepairTypeId);
        ViewData["ComplexityLevelId"] = new SelectList(_context.ComplexityLevels.OrderBy(x => x.Name), "Id", "Name", priceListItem?.ComplexityLevelId);
        return View(priceListItem);
    }

    // GET: PriceListItems/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var priceListItem = await _context.PriceListItems.FindAsync(id);
        if (priceListItem == null)
        {
            return NotFound();
        }
        ViewData["DeviceTypeId"] = new SelectList(_context.DeviceTypes.OrderBy(x => x.Name), "Id", "Name", priceListItem?.DeviceTypeId);
        ViewData["RepairTypeId"] = new SelectList(_context.RepairTypes.OrderBy(x => x.Name), "Id", "Name", priceListItem?.RepairTypeId);
        ViewData["ComplexityLevelId"] = new SelectList(_context.ComplexityLevels.OrderBy(x => x.Name), "Id", "Name", priceListItem?.ComplexityLevelId);
        return View(priceListItem);
    }

    // POST: PriceListItems/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,DeviceTypeId,RepairTypeId,ComplexityLevelId,Price,DurationMinutes")] PriceListItem priceListItem)
    {
        if (id != priceListItem.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(priceListItem);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PriceListItemExists(priceListItem.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        ViewData["DeviceTypeId"] = new SelectList(_context.DeviceTypes.OrderBy(x => x.Name), "Id", "Name", priceListItem?.DeviceTypeId);
        ViewData["RepairTypeId"] = new SelectList(_context.RepairTypes.OrderBy(x => x.Name), "Id", "Name", priceListItem?.RepairTypeId);
        ViewData["ComplexityLevelId"] = new SelectList(_context.ComplexityLevels.OrderBy(x => x.Name), "Id", "Name", priceListItem?.ComplexityLevelId);
        return View(priceListItem);
    }

    // GET: PriceListItems/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var priceListItem = await _context.PriceListItems
            .Include(x => x.DeviceType)
            .Include(x => x.RepairType)
            .Include(x => x.ComplexityLevel)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (priceListItem == null)
        {
            return NotFound();
        }

        return View(priceListItem);
    }

    // POST: PriceListItems/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var priceListItem = await _context.PriceListItems
            .Include(x => x.DeviceType)
            .Include(x => x.RepairType)
            .Include(x => x.ComplexityLevel)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (priceListItem == null)
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            _context.PriceListItems.Remove(priceListItem);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ViewData["Error"] = "This record cannot be deleted because other data depends on it.";
            return View(priceListItem);
        }

        return RedirectToAction(nameof(Index));
    }

    private bool PriceListItemExists(int id)
    {
        return _context.PriceListItems.Any(e => e.Id == id);
    }
}
