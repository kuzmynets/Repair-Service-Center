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
public class DeviceTypesController : Controller
{
    private readonly RepairServiceContext _context;
    // Laboratory work 8: messages of the controller in the language of the user
    private readonly IStringLocalizer<SharedResource> _localizer;

    public DeviceTypesController(RepairServiceContext context,
        IStringLocalizer<SharedResource> localizer)
    {
        _localizer = localizer;
        _context = context;
    }

    // GET: DeviceTypes
    public async Task<IActionResult> Index()
    {
        var items = await _context.DeviceTypes
            .OrderBy(x => x.Name)
            .ToListAsync();
        return View(items);
    }

    // GET: DeviceTypes/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var deviceType = await _context.DeviceTypes
            .FirstOrDefaultAsync(m => m.Id == id);
        if (deviceType == null)
        {
            return NotFound();
        }

        return View(deviceType);
    }

    // GET: DeviceTypes/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: DeviceTypes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name")] DeviceType deviceType)
    {
        if (ModelState.IsValid)
        {
            _context.Add(deviceType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(deviceType);
    }

    // GET: DeviceTypes/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var deviceType = await _context.DeviceTypes.FindAsync(id);
        if (deviceType == null)
        {
            return NotFound();
        }
        return View(deviceType);
    }

    // POST: DeviceTypes/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] DeviceType deviceType)
    {
        if (id != deviceType.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(deviceType);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceTypeExists(deviceType.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(deviceType);
    }

    // GET: DeviceTypes/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var deviceType = await _context.DeviceTypes
            .FirstOrDefaultAsync(m => m.Id == id);
        if (deviceType == null)
        {
            return NotFound();
        }

        return View(deviceType);
    }

    // POST: DeviceTypes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var deviceType = await _context.DeviceTypes
            .FirstOrDefaultAsync(m => m.Id == id);
        if (deviceType == null)
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            _context.DeviceTypes.Remove(deviceType);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ViewData["Error"] = _localizer["This device type is used in the price list and cannot be deleted."].Value;
            return View(deviceType);
        }

        return RedirectToAction(nameof(Index));
    }

    private bool DeviceTypeExists(int id)
    {
        return _context.DeviceTypes.Any(e => e.Id == id);
    }
}
