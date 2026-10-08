using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

    // GET: /Services?searchString=screen&deviceTypeId=1&repairTypeId=2&complexityLevelId=1&minPrice=100&maxPrice=2000&sort=price
    [HttpGet]
    public async Task<IActionResult> Index(string? searchString, int? deviceTypeId, int? repairTypeId,
        int? complexityLevelId, decimal? minPrice, decimal? maxPrice, string? sort)
    {
        // If the user mixed up the prices, swap them
        if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
        {
            (minPrice, maxPrice) = (maxPrice, minPrice);
        }

        // The query is only built here; it is sent to the database in ToListAsync()
        IQueryable<PriceListItem> query = _context.PriceListItems
            .Include(p => p.DeviceType)
            .Include(p => p.RepairType)
            .Include(p => p.ComplexityLevel);

        // Text search in several fields (SQL LIKE, not case-sensitive in SQL Server)
        if (!string.IsNullOrWhiteSpace(searchString))
        {
            var text = searchString.Trim();
            query = query.Where(p =>
                p.RepairType!.Name.Contains(text) ||
                (p.RepairType.Description != null && p.RepairType.Description.Contains(text)) ||
                p.DeviceType!.Name.Contains(text) ||
                p.ComplexityLevel!.Name.Contains(text));
        }

        // Filters by the drop-down lists
        if (deviceTypeId.HasValue)
        {
            query = query.Where(p => p.DeviceTypeId == deviceTypeId.Value);
        }
        if (repairTypeId.HasValue)
        {
            query = query.Where(p => p.RepairTypeId == repairTypeId.Value);
        }
        if (complexityLevelId.HasValue)
        {
            query = query.Where(p => p.ComplexityLevelId == complexityLevelId.Value);
        }

        // Price range
        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }
        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        // Sort order; "popular" = the number of orders with this service
        query = sort switch
        {
            "price_desc" => query.OrderByDescending(p => p.Price),
            "duration" => query.OrderBy(p => p.DurationMinutes),
            "popular" => query.OrderByDescending(p => _context.OrderItems.Count(i => i.PriceListItemId == p.Id))
                              .ThenBy(p => p.Price),
            _ => query.OrderBy(p => p.Price)
        };

        var deviceTypes = await _context.DeviceTypes.OrderBy(d => d.Name).ToListAsync();

        var model = new ServiceCatalogViewModel
        {
            Services = await query.ToListAsync(),
            DeviceTypes = deviceTypes,
            DeviceTypeList = new SelectList(deviceTypes, "Id", "Name", deviceTypeId),
            RepairTypeList = new SelectList(
                await _context.RepairTypes.OrderBy(r => r.Name).ToListAsync(), "Id", "Name", repairTypeId),
            ComplexityLevelList = new SelectList(
                await _context.ComplexityLevels.OrderBy(c => c.Name).ToListAsync(), "Id", "Name", complexityLevelId),
            SortList = new SelectList(new[]
            {
                new { Value = "price", Text = "Price: low to high" },
                new { Value = "price_desc", Text = "Price: high to low" },
                new { Value = "duration", Text = "Fastest first" },
                new { Value = "popular", Text = "Most popular" }
            }, "Value", "Text", sort ?? "price"),
            SearchString = searchString,
            SelectedDeviceTypeId = deviceTypeId,
            RepairTypeId = repairTypeId,
            ComplexityLevelId = complexityLevelId,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
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
