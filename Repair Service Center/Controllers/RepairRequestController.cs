using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Repair_Service_Center.Data;
using Repair_Service_Center.Models;
using Repair_Service_Center.Models.ViewModels;

namespace Repair_Service_Center.Controllers;

/// <summary>
/// Public form where a customer submits a repair request.
/// </summary>
public class RepairRequestController : Controller
{
    private readonly RepairServiceContext _context;

    public RepairRequestController(RepairServiceContext context)
    {
        _context = context;
    }

    // GET: /RepairRequest/Create?serviceId=5
    [HttpGet]
    public async Task<IActionResult> Create(int? serviceId)
    {
        var model = new RepairRequestViewModel { PriceListItemId = serviceId };
        await FillServicesAsync(model);
        return View(model);
    }

    // POST: /RepairRequest/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RepairRequestViewModel model)
    {
        PriceListItem? service = null;

        if (model.PriceListItemId.HasValue)
        {
            service = await _context.PriceListItems
                .Include(p => p.DeviceType)
                .Include(p => p.RepairType)
                .Include(p => p.ComplexityLevel)
                .FirstOrDefaultAsync(p => p.Id == model.PriceListItemId.Value);

            if (service == null)
            {
                ModelState.AddModelError(nameof(model.PriceListItemId), "The selected service does not exist.");
            }
        }
        else if (string.IsNullOrWhiteSpace(model.ProblemDescription))
        {
            // Without a service the customer must describe the problem
            ModelState.AddModelError(nameof(model.ProblemDescription),
                "Choose a service or describe the problem.");
        }

        if (!ModelState.IsValid)
        {
            await FillServicesAsync(model);
            return View(model);
        }

        var order = new Order
        {
            CustomerName = model.CustomerName.Trim(),
            CustomerPhone = model.CustomerPhone.Trim(),
            CreatedAt = DateTime.Now,
            // Standard request -> price is known; custom request -> waits for the administrator
            Status = service != null ? OrderStatus.Accepted : OrderStatus.Pending,
            TotalCost = service?.Price,
            ProblemDescription = BuildDescription(service, model.ProblemDescription)
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Confirmation), new { id = order.Id });
    }

    // GET: /RepairRequest/Confirmation/7
    [HttpGet]
    public async Task<IActionResult> Confirmation(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _context.Orders
            .Include(o => o.Technician)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    /// <summary>
    /// Fills the drop-down list with services from the price list.
    /// </summary>
    private async Task FillServicesAsync(RepairRequestViewModel model)
    {
        var services = await _context.PriceListItems
            .Include(p => p.DeviceType)
            .Include(p => p.RepairType)
            .Include(p => p.ComplexityLevel)
            .OrderBy(p => p.DeviceType!.Name)
            .ThenBy(p => p.Price)
            .ToListAsync();

        model.Services = services.Select(p => new SelectListItem
        {
            Value = p.Id.ToString(),
            Text = $"{p.DeviceType!.Name} \u2013 {p.RepairType!.Name} ({p.ComplexityLevel!.Name}) \u2013 {p.Price:0} UAH",
            Selected = p.Id == model.PriceListItemId
        }).ToList();
    }

    private static string? BuildDescription(PriceListItem? service, string? problem)
    {
        if (service == null)
        {
            return problem?.Trim();
        }

        var text = $"Service: {service.DeviceType!.Name} \u2013 {service.RepairType!.Name} ({service.ComplexityLevel!.Name})";
        return string.IsNullOrWhiteSpace(problem) ? text : $"{text}. {problem.Trim()}";
    }
}
