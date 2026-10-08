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
        await FillListsAsync(model);
        return View(model);
    }

    // POST: /RepairRequest/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RepairRequestViewModel model)
    {
        PriceListItem? service = null;
        Slot? slot = null;

        if (model.PriceListItemId.HasValue)
        {
            // Standard request: a service from the price list + a free time slot
            service = await _context.PriceListItems
                .FirstOrDefaultAsync(p => p.Id == model.PriceListItemId.Value);

            if (service == null)
            {
                ModelState.AddModelError(nameof(model.PriceListItemId), "The selected service does not exist.");
            }

            if (!model.SlotId.HasValue)
            {
                ModelState.AddModelError(nameof(model.SlotId), "Please choose a convenient time.");
            }
            else
            {
                slot = await _context.Slots.FirstOrDefaultAsync(s => s.Id == model.SlotId.Value);

                // The slot must exist, be free and be in the future
                if (slot == null || slot.IsBooked || slot.StartTime <= DateTime.Now)
                {
                    ModelState.AddModelError(nameof(model.SlotId),
                        "This time is already taken. Please choose another time.");
                }
            }
        }
        else if (string.IsNullOrWhiteSpace(model.ProblemDescription))
        {
            // Custom request: the customer must describe the problem
            ModelState.AddModelError(nameof(model.ProblemDescription),
                "Choose a service or describe the problem.");
        }

        if (!ModelState.IsValid)
        {
            await FillListsAsync(model);
            return View(model);
        }

        var order = new Order
        {
            CustomerName = model.CustomerName.Trim(),
            CustomerPhone = model.CustomerPhone.Trim(),
            CustomerEmail = string.IsNullOrWhiteSpace(model.CustomerEmail) ? null : model.CustomerEmail.Trim(),
            DeviceBrand = string.IsNullOrWhiteSpace(model.DeviceBrand) ? null : model.DeviceBrand.Trim(),
            DeviceModel = string.IsNullOrWhiteSpace(model.DeviceModel) ? null : model.DeviceModel.Trim(),
            CreatedAt = DateTime.Now,
            // Standard request -> price is known; custom request -> waits for the administrator
            Status = service != null ? OrderStatus.Accepted : OrderStatus.Pending,
            TotalCost = service?.Price,
            TechnicianId = slot?.TechnicianId,
            ProblemDescription = string.IsNullOrWhiteSpace(model.ProblemDescription)
                ? null
                : model.ProblemDescription.Trim()
        };

        if (service != null && slot != null)
        {
            // The price and the duration are copied from the price list
            order.Items.Add(new OrderItem
            {
                PriceListItemId = service.Id,
                SlotId = slot.Id,
                Price = service.Price,
                DurationMinutes = service.DurationMinutes
            });
            slot.IsBooked = true;
        }

        order.StatusHistory.Add(new OrderStatusHistory
        {
            Status = order.Status,
            ChangedAt = DateTime.Now,
            Comment = "Request created by the customer"
        });

        _context.Orders.Add(order);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // Two customers booked the same slot at the same moment:
            // the unique index on OrderItems.SlotId does not allow it
            _context.ChangeTracker.Clear();
            ModelState.AddModelError(nameof(model.SlotId),
                "This time has just been booked by another customer. Please choose another time.");
            await FillListsAsync(model);
            return View(model);
        }

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
            .Include(o => o.Items).ThenInclude(i => i.Slot)
            .Include(o => o.Items).ThenInclude(i => i.PriceListItem!).ThenInclude(p => p.DeviceType)
            .Include(o => o.Items).ThenInclude(i => i.PriceListItem!).ThenInclude(p => p.RepairType)
            .Include(o => o.Items).ThenInclude(i => i.PriceListItem!).ThenInclude(p => p.ComplexityLevel)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    // GET: /RepairRequest/Track?orderId=7&phone=0501112233
    // The customer finds the request by its number and the phone number
    [HttpGet]
    public async Task<IActionResult> Track(int? orderId, string? phone)
    {
        var model = new TrackRequestViewModel { OrderId = orderId, Phone = phone };

        if (orderId.HasValue && !string.IsNullOrWhiteSpace(phone))
        {
            model.Searched = true;

            var order = await _context.Orders
                .Include(o => o.Technician)
                .Include(o => o.StatusHistory)
                .Include(o => o.Items).ThenInclude(i => i.Slot)
                .Include(o => o.Items).ThenInclude(i => i.PriceListItem!).ThenInclude(p => p.DeviceType)
                .Include(o => o.Items).ThenInclude(i => i.PriceListItem!).ThenInclude(p => p.RepairType)
                .FirstOrDefaultAsync(o => o.Id == orderId.Value);

            // The phone is compared only by digits, so "+38 050 111-22-33" = "0501112233"
            if (order != null && PhoneMatches(order.CustomerPhone, phone))
            {
                model.Order = order;
            }
        }

        return View(model);
    }

    private static bool PhoneMatches(string stored, string entered)
    {
        var a = new string(stored.Where(char.IsDigit).ToArray());
        var b = new string(entered.Where(char.IsDigit).ToArray());

        // The last 9 digits are enough: 050 111 22 33 and +380 50 111 22 33 are the same number
        return b.Length >= 9 && a.Length >= 9 && a[^9..] == b[^9..];
    }

    /// <summary>
    /// Fills the drop-down lists: services from the price list and free time slots.
    /// </summary>
    private async Task FillListsAsync(RepairRequestViewModel model)
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

        var now = DateTime.Now;
        var slots = await _context.Slots
            .Include(s => s.Technician)
            .Where(s => !s.IsBooked && s.StartTime > now && s.StartTime < now.AddDays(14))
            .OrderBy(s => s.StartTime)
            .ThenBy(s => s.Technician!.FullName)
            .Take(60)
            .ToListAsync();

        model.Slots = slots.Select(s => new SelectListItem
        {
            Value = s.Id.ToString(),
            Text = $"{s.TimeText} \u00b7 {s.Technician!.FullName}",
            Selected = s.Id == model.SlotId
        }).ToList();
    }
}
