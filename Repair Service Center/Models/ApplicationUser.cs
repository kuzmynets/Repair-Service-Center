using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Repair_Service_Center.Models;

/// <summary>
/// User of the site: customer, moderator or administrator.
/// IdentityUser already has Id, UserName, Email, PhoneNumber, PasswordHash and other fields,
/// here we only add the full name and the link to the requests.
/// </summary>
public class ApplicationUser : IdentityUser
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    // Requests created by this user
    public List<Order> Orders { get; set; } = new();
}
