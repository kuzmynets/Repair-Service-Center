using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repair_Service_Center.Data;
using Repair_Service_Center.Models;

namespace Repair_Service_Center.Controllers;

/// <summary>
/// "My requests" of the authorized customer. A customer sees only his own requests.
/// </summary>
[Authorize(Roles = AppRoles.Customer)]
public class MyOrdersController : Controller
{
    private readonly RepairServiceContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public MyOrdersController(RepairServiceContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: /MyOrders
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);

        var orders = await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.Items).ThenInclude(i => i.PriceListItem!).ThenInclude(p => p.DeviceType)
            .Include(o => o.Items).ThenInclude(i => i.PriceListItem!).ThenInclude(p => p.RepairType)
            .Include(o => o.Items).ThenInclude(i => i.Slot)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return View(orders);
    }

    // GET: /MyOrders/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var userId = _userManager.GetUserId(User);

        // The condition UserId == userId: a customer cannot open a request of another customer
        var order = await _context.Orders
            .Where(o => o.Id == id && o.UserId == userId)
            .Include(o => o.Technician)
            .Include(o => o.StatusHistory)
            .Include(o => o.Items).ThenInclude(i => i.Slot)
            .Include(o => o.Items).ThenInclude(i => i.PriceListItem!).ThenInclude(p => p.DeviceType)
            .Include(o => o.Items).ThenInclude(i => i.PriceListItem!).ThenInclude(p => p.RepairType)
            .FirstOrDefaultAsync();

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }
}
