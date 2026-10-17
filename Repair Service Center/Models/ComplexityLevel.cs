using System.ComponentModel.DataAnnotations;
using Repair_Service_Center.Validation;

namespace Repair_Service_Center.Models;

/// <summary>
/// Complexity class of the device (budget, mid-range, premium).
/// </summary>
public class ComplexityLevel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Please enter the name.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "The field {0} must be from {2} to {1} characters long.")]
    // Laboratory work 7: the built-in RegularExpression attribute (as Genre in the methodical example)
    [RegularExpression(ValidationPatterns.Title, ErrorMessage = ValidationPatterns.TitleMessage)]
    [Display(Name = "Complexity level")]
    public string Name { get; set; } = string.Empty;
}
