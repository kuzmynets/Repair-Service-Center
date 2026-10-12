using System.ComponentModel.DataAnnotations;
using Repair_Service_Center.Validation;

namespace Repair_Service_Center.Models;

/// <summary>
/// Type of repair work (screen replacement, cleaning, diagnostics, etc.).
/// </summary>
public class RepairType
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Please enter the name.")]
    [StringLength(100, MinimumLength = 2)]
    // Laboratory work 7: the built-in RegularExpression attribute (as Genre in the methodical example)
    [RegularExpression(ValidationPatterns.Title, ErrorMessage = ValidationPatterns.TitleMessage)]
    [Display(Name = "Repair type")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}
