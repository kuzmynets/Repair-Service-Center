using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repair_Service_Center.Data;
using Repair_Service_Center.Models;
using Repair_Service_Center.Models.ViewModels;

namespace Repair_Service_Center.Controllers;

/// <summary>
/// The administrator sees all users and gives them roles (for example, makes a user a moderator).
/// Access is described by the policy RequireAdmin (see Program.cs).
/// </summary>
[Authorize(Policy = AppPolicies.RequireAdmin)]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RepairServiceContext _context;

    public UsersController(UserManager<ApplicationUser> userManager, RepairServiceContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    // GET: /Users
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var currentId = _userManager.GetUserId(User);
        var users = await _userManager.Users.OrderBy(u => u.Email).ToListAsync();

        // Number of requests of every user (one query for all users)
        var ordersCount = await _context.Orders
            .Where(o => o.UserId != null)
            .GroupBy(o => o.UserId!)
            .Select(g => new { UserId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.UserId, x => x.Count);

        var model = new List<UserRoleViewModel>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            model.Add(new UserRoleViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                Role = roles.FirstOrDefault() ?? "-",
                OrdersCount = ordersCount.GetValueOrDefault(user.Id),
                IsCurrentUser = user.Id == currentId
            });
        }

        return View(model);
    }

    // POST: /Users/ChangeRole - one role for one user
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeRole(string id, string role)
    {
        if (!AppRoles.All.Contains(role))
        {
            return BadRequest();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        // The administrator cannot take away his own rights by mistake
        if (user.Id == _userManager.GetUserId(User))
        {
            TempData["Error"] = "You cannot change your own role.";
            return RedirectToAction(nameof(Index));
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, role);

        // The old login cookie of the user becomes invalid, so he gets the new rights
        // after the next login (at the latest in 30 minutes)
        await _userManager.UpdateSecurityStampAsync(user);

        TempData["Message"] = $"{user.Email} now has the role {role}.";
        return RedirectToAction(nameof(Index));
    }
}
