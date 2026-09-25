using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Repair_Service_Center.Models.ViewModels;

/// <summary>
/// Form "Request a repair" for customers.
/// The customer chooses a service from the price list or describes the problem.
/// </summary>
public class RepairRequestViewModel
{
    [Required(ErrorMessage = "Please enter your name.")]
    [StringLength(100, MinimumLength = 3)]
    [Display(Name = "Your name")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your phone number.")]
    [Phone]
    [StringLength(20)]
    [Display(Name = "Phone number")]
    public string CustomerPhone { get; set; } = string.Empty;

    [Display(Name = "Service")]
    public int? PriceListItemId { get; set; }

    [StringLength(800)]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Describe the problem")]
    public string? ProblemDescription { get; set; }

    // Items for the drop-down list. They are not sent by the form, so no validation.
    [ValidateNever]
    public IEnumerable<SelectListItem> Services { get; set; } = new List<SelectListItem>();
}
