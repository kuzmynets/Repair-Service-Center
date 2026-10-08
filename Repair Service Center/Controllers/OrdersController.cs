using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Repair_Service_Center.Data;
using Repair_Service_Center.Models;
using Repair_Service_Center.Models.ViewModels;

namespace Repair_Service_Center.Controllers;

public class OrdersController : Controller
{
    private readonly RepairServiceContext _context;

    public OrdersController(RepairServiceContext context)
    {
        _context = context;
    }

    // GET: Orders?searchString=samsung&status=2&technicianId=1&dateFrom=2026-10-01&dateTo=2026-10-31
    public async Task<IActionResult> Index(string? searchString, OrderStatus? status, int? technicianId,
        DateTime? dateFrom, DateTime? dateTo)
    {
        IQueryable<Order> orders = _context.Orders.Include(x => x.Technician);

        // One field searches by customer name, phone, email, device brand and model,
        // or by the request number if the text is a number
        if (!string.IsNullOrWhiteSpace(searchString))
        {
            var text = searchString.Trim();
            var isNumber = int.TryParse(text.TrimStart('#'), out var orderId);

            orders = orders.Where(o =>
                (isNumber && o.Id == orderId) ||
                o.CustomerName.Contains(text) ||
                o.CustomerPhone.Contains(text) ||
                (o.CustomerEmail != null && o.CustomerEmail.Contains(text)) ||
                (o.DeviceBrand != null && o.DeviceBrand.Contains(text)) ||
                (o.DeviceModel != null && o.DeviceModel.Contains(text)));
        }

        if (status.HasValue)
        {
            orders = orders.Where(o => o.Status == status.Value);
        }

        if (technicianId.HasValue)
        {
            orders = orders.Where(o => o.TechnicianId == technicianId.Value);
        }

        // Date range (the end date is included in the result)
        if (dateFrom.HasValue)
        {
            orders = orders.Where(o => o.CreatedAt >= dateFrom.Value.Date);
        }
        if (dateTo.HasValue)
        {
            var end = dateTo.Value.Date.AddDays(1);
            orders = orders.Where(o => o.CreatedAt < end);
        }

        var model = new OrderSearchViewModel
        {
            Orders = await orders.OrderByDescending(o => o.CreatedAt).ToListAsync(),
            Technicians = new SelectList(
                await _context.Technicians.OrderBy(t => t.FullName).ToListAsync(), "Id", "FullName", technicianId),
            SearchString = searchString,
            Status = status,
            TechnicianId = technicianId,
            DateFrom = dateFrom,
            DateTo = dateTo
        };

        return View(model);
    }

    // GET: Orders/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        // Order with its items (service + time slot) and the history of statuses
        var order = await _context.Orders
            .Include(x => x.Technician)
            .Include(x => x.Items).ThenInclude(i => i.Slot)
            .Include(x => x.Items).ThenInclude(i => i.PriceListItem!).ThenInclude(p => p.DeviceType)
            .Include(x => x.Items).ThenInclude(i => i.PriceListItem!).ThenInclude(p => p.RepairType)
            .Include(x => x.Items).ThenInclude(i => i.PriceListItem!).ThenInclude(p => p.ComplexityLevel)
            .Include(x => x.StatusHistory)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    // GET: Orders/Create
    public IActionResult Create()
    {
        ViewData["TechnicianId"] = new SelectList(_context.Technicians.OrderBy(x => x.FullName), "Id", "FullName");
        return View();
    }

    // POST: Orders/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,CustomerName,CustomerPhone,CustomerEmail,DeviceBrand,DeviceModel,Status,ProblemDescription,TotalCost,TechnicianId")] Order order)
    {
        order.CreatedAt = DateTime.Now;

        if (ModelState.IsValid)
        {
            order.StatusHistory.Add(new OrderStatusHistory
            {
                Status = order.Status,
                ChangedAt = DateTime.Now,
                Comment = "Request created by the administrator"
            });
            _context.Add(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["TechnicianId"] = new SelectList(_context.Technicians.OrderBy(x => x.FullName), "Id", "FullName", order?.TechnicianId);
        return View(order);
    }

    // GET: Orders/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            return NotFound();
        }
        ViewData["TechnicianId"] = new SelectList(_context.Technicians.OrderBy(x => x.FullName), "Id", "FullName", order?.TechnicianId);
        return View(order);
    }

    // POST: Orders/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerName,CustomerPhone,CustomerEmail,DeviceBrand,DeviceModel,Status,ProblemDescription,TotalCost,TechnicianId")] Order order)
    {
        if (id != order.Id)
        {
            return NotFound();
        }

        // The order is loaded from the database, so the creation date and items are not lost
        var existing = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (existing == null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            // A new record in the history only when the status is changed
            if (existing.Status != order.Status)
            {
                _context.OrderStatusHistory.Add(new OrderStatusHistory
                {
                    OrderId = existing.Id,
                    Status = order.Status,
                    ChangedAt = DateTime.Now,
                    Comment = $"Status changed from {existing.Status.GetDisplayName()}"
                });
            }

            existing.CustomerName = order.CustomerName;
            existing.CustomerPhone = order.CustomerPhone;
            existing.CustomerEmail = order.CustomerEmail;
            existing.DeviceBrand = order.DeviceBrand;
            existing.DeviceModel = order.DeviceModel;
            existing.Status = order.Status;
            existing.ProblemDescription = order.ProblemDescription;
            existing.TotalCost = order.TotalCost;
            existing.TechnicianId = order.TechnicianId;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        order.CreatedAt = existing.CreatedAt;
        ViewData["TechnicianId"] = new SelectList(_context.Technicians.OrderBy(x => x.FullName), "Id", "FullName", order?.TechnicianId);
        return View(order);
    }

    // GET: Orders/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _context.Orders
            .Include(x => x.Technician)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    // POST: Orders/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var order = await _context.Orders
            .Include(x => x.Technician)
            .Include(x => x.Items).ThenInclude(i => i.Slot)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (order == null)
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            // The booked time slots become free again
            foreach (var item in order.Items)
            {
                if (item.Slot != null)
                {
                    item.Slot.IsBooked = false;
                }
            }

            // Items and history are deleted by cascade
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ViewData["Error"] = "This record cannot be deleted because other data depends on it.";
            return View(order);
        }

        return RedirectToAction(nameof(Index));
    }

    private bool OrderExists(int id)
    {
        return _context.Orders.Any(e => e.Id == id);
    }
}
