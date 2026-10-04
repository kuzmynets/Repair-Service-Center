using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Repair_Service_Center.Data;
using Repair_Service_Center.Models;

namespace Repair_Service_Center.Controllers;

/// <summary>
/// Administrator: schedule (time slots) of the technicians.
/// </summary>
public class SlotsController : Controller
{
    private readonly RepairServiceContext _context;

    public SlotsController(RepairServiceContext context)
    {
        _context = context;
    }

    // GET: Slots?technicianId=1&showPast=false
    [HttpGet]
    public async Task<IActionResult> Index(int? technicianId, bool showPast = false)
    {
        IQueryable<Slot> query = _context.Slots.Include(s => s.Technician);

        if (!showPast)
        {
            query = query.Where(s => s.EndTime >= DateTime.Now);
        }

        if (technicianId.HasValue)
        {
            query = query.Where(s => s.TechnicianId == technicianId.Value);
        }

        var slots = await query
            .OrderBy(s => s.StartTime)
            .ThenBy(s => s.Technician!.FullName)
            .ToListAsync();

        ViewData["TechnicianFilter"] = new SelectList(
            await _context.Technicians.OrderBy(t => t.FullName).ToListAsync(), "Id", "FullName", technicianId);
        ViewData["ShowPast"] = showPast;

        return View(slots);
    }

    // GET: Slots/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var slot = await _context.Slots
            .Include(s => s.Technician)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (slot == null)
        {
            return NotFound();
        }

        // Which order uses this slot (if it is booked)
        ViewData["OrderId"] = await _context.OrderItems
            .Where(i => i.SlotId == slot.Id)
            .Select(i => (int?)i.OrderId)
            .FirstOrDefaultAsync();

        return View(slot);
    }

    // GET: Slots/Create
    [HttpGet]
    public IActionResult Create()
    {
        // Default values: tomorrow 10:00-12:00
        var start = DateTime.Today.AddDays(1).AddHours(10);
        var slot = new Slot { StartTime = start, EndTime = start.AddHours(2) };
        FillTechnicians(slot.TechnicianId);
        return View(slot);
    }

    // POST: Slots/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("TechnicianId,StartTime,EndTime")] Slot slot)
    {
        if (slot.StartTime <= DateTime.Now)
        {
            ModelState.AddModelError(nameof(slot.StartTime), "Start time must be in the future.");
        }
        await CheckOverlapAsync(slot);

        if (ModelState.IsValid)
        {
            _context.Add(slot);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        FillTechnicians(slot.TechnicianId);
        return View(slot);
    }

    // GET: Slots/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var slot = await _context.Slots.FindAsync(id);
        if (slot == null)
        {
            return NotFound();
        }
        FillTechnicians(slot.TechnicianId);
        return View(slot);
    }

    // POST: Slots/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,TechnicianId,StartTime,EndTime,IsBooked")] Slot slot)
    {
        if (id != slot.Id)
        {
            return NotFound();
        }

        await CheckOverlapAsync(slot);

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(slot);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SlotExists(slot.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        FillTechnicians(slot.TechnicianId);
        return View(slot);
    }

    // GET: Slots/Delete/5
    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var slot = await _context.Slots
            .Include(s => s.Technician)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (slot == null)
        {
            return NotFound();
        }

        return View(slot);
    }

    // POST: Slots/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var slot = await _context.Slots
            .Include(s => s.Technician)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (slot == null)
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            _context.Slots.Remove(slot);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ViewData["Error"] = "This time slot is booked in an order and cannot be deleted.";
            return View(slot);
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Slots/Generate - adds slots for the next 7 days for all technicians
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Generate()
    {
        var added = SeedData.AddSlots(_context, DateTime.Today.AddDays(1), 7);
        TempData["Message"] = added > 0
            ? $"{added} new time slots were added."
            : "The schedule for the next week is already full.";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// One technician cannot have two slots that overlap in time.
    /// </summary>
    private async Task CheckOverlapAsync(Slot slot)
    {
        var overlap = await _context.Slots.AnyAsync(s =>
            s.Id != slot.Id &&
            s.TechnicianId == slot.TechnicianId &&
            s.StartTime < slot.EndTime &&
            slot.StartTime < s.EndTime);

        if (overlap)
        {
            ModelState.AddModelError(string.Empty, "This technician already has a time slot at this time.");
        }
    }

    private void FillTechnicians(int selectedId)
    {
        ViewData["TechnicianId"] = new SelectList(
            _context.Technicians.OrderBy(t => t.FullName), "Id", "FullName", selectedId);
    }

    private bool SlotExists(int id)
    {
        return _context.Slots.Any(e => e.Id == id);
    }
}
