using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repair_Service_Center.Data;
using Repair_Service_Center.Models;

namespace Repair_Service_Center.Controllers;

/// <summary>
/// Customer reviews: public list and form, and moderation.
/// (In laboratory work 6 the moderation will be allowed only for the Moderator role.)
/// </summary>
public class ReviewsController : Controller
{
    private readonly RepairServiceContext _context;

    public ReviewsController(RepairServiceContext context)
    {
        _context = context;
    }

    // GET: /Reviews - only approved reviews
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var reviews = await _context.Reviews
            .Where(r => r.IsApproved)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        ViewData["Average"] = reviews.Count > 0 ? reviews.Average(r => r.Rating) : 0;
        return View(reviews);
    }

    // GET: /Reviews/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Review { Rating = 5 });
    }

    // POST: /Reviews/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("AuthorName,Rating,Text,OrderId")] Review review)
    {
        if (review.OrderId.HasValue && !await _context.Orders.AnyAsync(o => o.Id == review.OrderId.Value))
        {
            ModelState.AddModelError(nameof(review.OrderId), "A request with this number was not found.");
        }

        if (!ModelState.IsValid)
        {
            return View(review);
        }

        review.CreatedAt = DateTime.Now;
        review.IsApproved = false; // the review waits for the moderator

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        TempData["Message"] = "Thank you! Your review will appear after moderation.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Reviews/Moderate - all reviews, new (not approved) first
    [HttpGet]
    public async Task<IActionResult> Moderate()
    {
        var reviews = await _context.Reviews
            .OrderBy(r => r.IsApproved)
            .ThenByDescending(r => r.CreatedAt)
            .ToListAsync();

        return View(reviews);
    }

    // POST: /Reviews/SetApproved/5?approved=true - approve or hide a review
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetApproved(int id, bool approved)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
        {
            return NotFound();
        }

        review.IsApproved = approved;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Moderate));
    }

    // POST: /Reviews/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review != null)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Moderate));
    }
}
