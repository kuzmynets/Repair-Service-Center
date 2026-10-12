using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repair_Service_Center.Validation;

namespace Repair_Service_Center.Models.ViewModels;

/// <summary>
/// Form "Request a repair" for customers.
/// The customer chooses a service from the price list or describes the problem.
/// </summary>
public class RepairRequestViewModel
{
    // Laboratory work 7: the same rules as in the model Order (Validation/AppValidationAttributes.cs)
    [Required(ErrorMessage = "Please enter your name.")]
    [StringLength(100, MinimumLength = 3)]
    [PersonName(2, 4)]
    [Display(Name = "Your name")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your phone number.")]
    [MobilePhone]
    [DataType(DataType.PhoneNumber)]
    [StringLength(20)]
    [Display(Name = "Mobile phone")]
    public string CustomerPhone { get; set; } = string.Empty;

    [EmailFormat]
    [DataType(DataType.EmailAddress)]
    [StringLength(100)]
    [Display(Name = "Email (optional)")]
    public string? CustomerEmail { get; set; }

    [StringLength(50)]
    [RegularExpression(ValidationPatterns.Device, ErrorMessage = ValidationPatterns.DeviceMessage)]
    [Display(Name = "Device brand")]
    public string? DeviceBrand { get; set; }

    [StringLength(50)]
    [RegularExpression(ValidationPatterns.Device, ErrorMessage = ValidationPatterns.DeviceMessage)]
    [Display(Name = "Device model")]
    public string? DeviceModel { get; set; }

    [StringLength(200)]
    [Address]
    [Display(Name = "Address (if the technician must come to you)")]
    public string? CustomerAddress { get; set; }

    [Display(Name = "Service")]
    public int? PriceListItemId { get; set; }

    // Time slot is chosen only for a service from the price list
    [Display(Name = "Convenient time")]
    public int? SlotId { get; set; }

    [StringLength(800, MinimumLength = 10, ErrorMessage = "The description must be from 10 to 800 characters.")]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Describe the problem")]
    public string? ProblemDescription { get; set; }

    // Items for the drop-down list. They are not sent by the form, so no validation.
    [ValidateNever]
    public IEnumerable<SelectListItem> Services { get; set; } = new List<SelectListItem>();

    [ValidateNever]
    public IEnumerable<SelectListItem> Slots { get; set; } = new List<SelectListItem>();
}
