using System.ComponentModel.DataAnnotations;

namespace Repair_Service_Center.Models;

/// <summary>
/// Technician who performs repairs. Managed by the administrator.
/// </summary>
public class Technician
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Specialization { get; set; } = string.Empty;

    [Phone]
    [StringLength(20)]
    public string? Phone { get; set; }
}
