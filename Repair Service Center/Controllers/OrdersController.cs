using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Repair_Service_Center.Data;
using Repair_Service_Center.Models;

namespace Repair_Service_Center.Controllers;

public class OrdersController : Controller
{
    private readonly RepairServiceContext _context;

    public OrdersController(RepairServiceContext context)
    {
        _context = context;
    }

    // GET: Orders
    public async Task<IActionResult> Index()
    {
        var items = await _context.Orders
            .Include(x => x.Technician)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
        return View(items);
    }

    // GET: Orders/Details/5
    public async Task<IActionResult> Details(int? id)
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

    // GET: Orders/Create
    public IActionResult Create()
    {
        ViewData["TechnicianId"] = new SelectList(_context.Technicians.OrderBy(x => x.FullName), "Id", "FullName");
        return View();
    }

    // POST: Orders/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,CustomerName,CustomerPhone,Status,ProblemDescription,TotalCost,TechnicianId")] Order order)
    {
        order.CreatedAt = DateTime.Now;

        if (ModelState.IsValid)
        {
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
    public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerName,CustomerPhone,CreatedAt,Status,ProblemDescription,TotalCost,TechnicianId")] Order order)
    {
        if (id != order.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(order);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(order.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
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
            .FirstOrDefaultAsync(m => m.Id == id);
        if (order == null)
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
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
