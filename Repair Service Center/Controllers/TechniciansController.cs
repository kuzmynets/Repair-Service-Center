using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Repair_Service_Center.Data;
using Repair_Service_Center.Models;

namespace Repair_Service_Center.Controllers;

// Laboratory work 6: only the administrator can open these pages
[Authorize(Roles = AppRoles.Admin)]
public class TechniciansController : Controller
{
    private readonly RepairServiceContext _context;
    // Laboratory work 8: messages of the controller in the language of the user
    private readonly IStringLocalizer<SharedResource> _localizer;

    public TechniciansController(RepairServiceContext context,
        IStringLocalizer<SharedResource> localizer)
    {
        _localizer = localizer;
        _context = context;
    }

    // GET: Technicians?searchString=iryna
    public async Task<IActionResult> Index(string? searchString)
    {
        var technicians = from t in _context.Technicians
                          select t;

        // Search by name, specialization or phone
        if (!string.IsNullOrWhiteSpace(searchString))
        {
            var text = searchString.Trim();
            technicians = technicians.Where(t =>
                t.FullName.Contains(text) ||
                t.Specialization.Contains(text) ||
                (t.Phone != null && t.Phone.Contains(text)));
        }

        ViewData["SearchString"] = searchString;
        return View(await technicians.OrderBy(t => t.FullName).ToListAsync());
    }

    // GET: Technicians/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var technician = await _context.Technicians
            .FirstOrDefaultAsync(m => m.Id == id);
        if (technician == null)
        {
            return NotFound();
        }

        return View(technician);
    }

    // GET: Technicians/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Technicians/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,FullName,Specialization,Phone,ExperienceYears")] Technician technician)
    {
        if (ModelState.IsValid)
        {
            _context.Add(technician);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(technician);
    }

    // GET: Technicians/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var technician = await _context.Technicians.FindAsync(id);
        if (technician == null)
        {
            return NotFound();
        }
        return View(technician);
    }

    // POST: Technicians/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Specialization,Phone,ExperienceYears")] Technician technician)
    {
        if (id != technician.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(technician);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TechnicianExists(technician.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(technician);
    }

    // GET: Technicians/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var technician = await _context.Technicians
            .FirstOrDefaultAsync(m => m.Id == id);
        if (technician == null)
        {
            return NotFound();
        }

        return View(technician);
    }

    // POST: Technicians/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var technician = await _context.Technicians
            .FirstOrDefaultAsync(m => m.Id == id);
        if (technician == null)
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            _context.Technicians.Remove(technician);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ViewData["Error"] = _localizer["This technician has booked time slots and cannot be deleted."].Value;
            return View(technician);
        }

        return RedirectToAction(nameof(Index));
    }

    private bool TechnicianExists(int id)
    {
        return _context.Technicians.Any(e => e.Id == id);
    }
}
