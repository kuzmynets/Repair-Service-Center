using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repair_Service_Center.Models;

namespace Repair_Service_Center.Data;

/// <summary>
/// Creates the roles and the demo accounts at application start (only if they do not exist).
/// Demo accounts (password rules: at least 8 characters, a digit and a small letter):
///   admin@example.com            / Admin2026  - Admin
///   moderator@example.com        / Moder2026  - Moderator
///   anna.shevchenko@example.com  / Anna2026   - Customer
/// </summary>
public static class IdentitySeed
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var context = services.GetRequiredService<RepairServiceContext>();

        // 1. Roles
        foreach (var role in AppRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 2. Demo users
        await CreateUserAsync(userManager, "admin@example.com", "Admin2026",
            "Site Administrator", "+380670000001", AppRoles.Admin);
        await CreateUserAsync(userManager, "moderator@example.com", "Moder2026",
            "Review Moderator", "+380670000002", AppRoles.Moderator);
        var anna = await CreateUserAsync(userManager, "anna.shevchenko@example.com", "Anna2026",
            "Anna Shevchenko", "+380501112233", AppRoles.Customer);

        // 3. The demo request of Anna (same e-mail) is linked to her account,
        //    so the page "My requests" is not empty
        if (anna != null)
        {
            var orders = await context.Orders
                .Where(o => o.UserId == null && o.CustomerEmail == anna.Email)
                .ToListAsync();

            if (orders.Count > 0)
            {
                orders.ForEach(o => o.UserId = anna.Id);
                await context.SaveChangesAsync();
            }
        }
    }

    private static async Task<ApplicationUser?> CreateUserAsync(UserManager<ApplicationUser> userManager,
        string email, string password, string fullName, string phone, string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user != null)
        {
            return user;
        }

        user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FullName = fullName,
            PhoneNumber = phone
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return null;
        }

        await userManager.AddToRoleAsync(user, role);
        return user;
    }
}
