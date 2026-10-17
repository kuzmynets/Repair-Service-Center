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
    [StringLength(100, MinimumLength = 2, ErrorMessage = "The field {0} must be from {2} to {1} characters long.")]
    // Laboratory work 7: the built-in RegularExpression attribute (as Genre in the methodical example)
    [RegularExpression(ValidationPatterns.Title, ErrorMessage = ValidationPatterns.TitleMessage)]
    [Display(Name = "Repair type")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "The field {0} must be at most {1} characters long.")]
    [Display(Name = "Description")]
    public string? Description { get; set; }
}
