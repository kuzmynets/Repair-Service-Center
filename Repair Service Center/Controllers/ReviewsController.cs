using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Repair_Service_Center.Data;
using Repair_Service_Center.Models;

namespace Repair_Service_Center.Controllers;

/// <summary>
/// Customer reviews: public list, form for customers and moderation.
/// Laboratory work 6: the controller has no [Authorize] on the class,
/// so every action has its own rule:
///   Index      - everybody (also guests);
///   Create     - only the Customer role;
///   Moderate, SetApproved, Delete - the policy CanModerateReviews (Moderator or Admin).
/// </summary>
public class ReviewsController : Controller
{
    private readonly RepairServiceContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    // Laboratory work 8: messages of the controller in the language of the user
    private readonly IStringLocalizer<SharedResource> _localizer;

    public ReviewsController(RepairServiceContext context, UserManager<ApplicationUser> userManager,
        IStringLocalizer<SharedResource> localizer)
    {
        _localizer = localizer;
        _context = context;
        _userManager = userManager;
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

    // GET: /Reviews/Create?orderId=5
    [HttpGet]
    [Authorize(Roles = AppRoles.Customer)]
    public async Task<IActionResult> Create(int? orderId)
    {
        // The name is taken from the profile of the customer
        var user = await _userManager.GetUserAsync(User);
        return View(new Review { Rating = 5, AuthorName = user?.FullName ?? string.Empty, OrderId = orderId });
    }

    // POST: /Reviews/Create
    [HttpPost]
    [Authorize(Roles = AppRoles.Customer)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("AuthorName,Rating,Text,OrderId")] Review review)
    {
        // A customer can write a review only about his own request
        var userId = _userManager.GetUserId(User);
        if (review.OrderId.HasValue &&
            !await _context.Orders.AnyAsync(o => o.Id == review.OrderId.Value && o.UserId == userId))
        {
            ModelState.AddModelError(nameof(review.OrderId), _localizer["You have no request with this number."].Value);
        }

        if (!ModelState.IsValid)
        {
            return View(review);
        }

        review.CreatedAt = DateTime.Now;
        review.IsApproved = false; // the review waits for the moderator

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        TempData["Message"] = _localizer["Thank you! Your review will appear after moderation."].Value;
        return RedirectToAction(nameof(Index));
    }

    // GET: /Reviews/Moderate - all reviews, new (not approved) first
    [HttpGet]
    [Authorize(Policy = AppPolicies.CanModerateReviews)]
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
    [Authorize(Policy = AppPolicies.CanModerateReviews)]
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
    [Authorize(Policy = AppPolicies.CanModerateReviews)]
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
