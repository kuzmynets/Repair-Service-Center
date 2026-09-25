using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repair_Service_Center.Data;
using Repair_Service_Center.Models;
using Repair_Service_Center.Models.ViewModels;

namespace Repair_Service_Center.Controllers;

/// <summary>
/// Public service catalog for customers (read only).
/// </summary>
public class ServicesController : Controller
{
    private readonly RepairServiceContext _context;

    public ServicesController(RepairServiceContext context)
    {
        _context = context;
    }

    // GET: /Services?deviceTypeId=1&sort=price_desc
    [HttpGet]
    public async Task<IActionResult> Index(int? deviceTypeId, string? sort)
    {
        IQueryable<PriceListItem> query = _context.PriceListItems
            .Include(p => p.DeviceType)
            .Include(p => p.RepairType)
            .Include(p => p.ComplexityLevel);

        // Filter by device type
        if (deviceTypeId.HasValue)
        {
            query = query.Where(p => p.DeviceTypeId == deviceTypeId.Value);
        }

        // Sort order
        query = sort switch
        {
            "price_desc" => query.OrderByDescending(p => p.Price),
            "duration" => query.OrderBy(p => p.DurationMinutes),
            _ => query.OrderBy(p => p.Price)
        };

        var model = new ServiceCatalogViewModel
        {
            Services = await query.ToListAsync(),
            DeviceTypes = await _context.DeviceTypes.OrderBy(d => d.Name).ToListAsync(),
            SelectedDeviceTypeId = deviceTypeId,
            Sort = sort ?? "price"
        };

        return View(model);
    }

    // GET: /Services/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var service = await _context.PriceListItems
            .Include(p => p.DeviceType)
            .Include(p => p.RepairType)
            .Include(p => p.ComplexityLevel)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (service == null)
        {
            return NotFound();
        }

        // Other services for the same device type
        var related = await _context.PriceListItems
            .Include(p => p.DeviceType)
            .Include(p => p.RepairType)
            .Include(p => p.ComplexityLevel)
            .Where(p => p.DeviceTypeId == service.DeviceTypeId && p.Id != service.Id)
            .OrderBy(p => p.Price)
            .Take(3)
            .ToListAsync();

        return View(new ServiceDetailsViewModel { Service = service, RelatedServices = related });
    }
}
