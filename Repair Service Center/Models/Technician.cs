using System.ComponentModel.DataAnnotations;
using Repair_Service_Center.Validation;

namespace Repair_Service_Center.Models;

/// <summary>
/// Technician who performs repairs. Managed by the administrator.
/// </summary>
public class Technician
{
    public int Id { get; set; }

    // Laboratory work 7: first and last name with capital letters
    [Required(ErrorMessage = "Please enter the full name.")]
    [StringLength(100, MinimumLength = 3)]
    [PersonName(2, 4)]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter the specialization.")]
    [StringLength(100)]
    [RegularExpression(ValidationPatterns.Title, ErrorMessage = ValidationPatterns.TitleMessage)]
    public string Specialization { get; set; } = string.Empty;

    [MobilePhone]
    [DataType(DataType.PhoneNumber)]
    [StringLength(20)]
    public string? Phone { get; set; }

    // Laboratory work 5: new field
    [Range(0, 50, ErrorMessage = "The experience must be from 0 to 50 years.")]
    [Display(Name = "Experience (years)")]
    public int ExperienceYears { get; set; }
}
