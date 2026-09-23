using System.ComponentModel.DataAnnotations;

namespace Repair_Service_Center.Models;

/// <summary>
/// Complexity class of the device (budget, mid-range, premium).
/// </summary>
public class ComplexityLevel
{
    public int Id { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 2)]
    [Display(Name = "Complexity level")]
    public string Name { get; set; } = string.Empty;
}
