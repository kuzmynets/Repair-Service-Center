using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Repair_Service_Center.Validation;

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

    // Laboratory work 7: address of the customer (used for a visit of the technician)
    [StringLength(200)]
    [Address]
    [Display(Name = "Address")]
    public string? Address { get; set; }

    // Requests created by this user
    public List<Order> Orders { get; set; } = new();
}
