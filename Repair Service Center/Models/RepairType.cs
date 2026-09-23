using System.ComponentModel.DataAnnotations;

namespace Repair_Service_Center.Models;

/// <summary>
/// Type of repair work (screen replacement, cleaning, diagnostics, etc.).
/// </summary>
public class RepairType
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    [Display(Name = "Repair type")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}
