using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Repair_Service_Center.Validation;

namespace Repair_Service_Center.Models;

/// <summary>
/// Repair request from a customer.
/// </summary>
public class Order
{
    public int Id { get; set; }

    // Laboratory work 7: own validation attributes (Validation/AppValidationAttributes.cs)
    [Required(ErrorMessage = "Please enter the name of the customer.")]
    [StringLength(100, MinimumLength = 3)]
    [PersonName(2, 4)]
    [Display(Name = "Customer")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter the phone number.")]
    [MobilePhone]
    [DataType(DataType.PhoneNumber)]
    [StringLength(20)]
    [Display(Name = "Phone")]
    public string CustomerPhone { get; set; } = string.Empty;

    // ----- Laboratory work 5: new fields -----
    [EmailFormat]
    [DataType(DataType.EmailAddress)]
    [StringLength(100)]
    [Display(Name = "Email")]
    public string? CustomerEmail { get; set; }

    [StringLength(50)]
    [RegularExpression(ValidationPatterns.Device, ErrorMessage = ValidationPatterns.DeviceMessage)]
    [Display(Name = "Brand")]
    public string? DeviceBrand { get; set; }

    [StringLength(50)]
    [RegularExpression(ValidationPatterns.Device, ErrorMessage = ValidationPatterns.DeviceMessage)]
    [Display(Name = "Model")]
    public string? DeviceModel { get; set; }

    // ----- Laboratory work 7: address for a visit of the technician (washing machines, refrigerators) -----
    [StringLength(200)]
    [Address]
    [Display(Name = "Address")]
    public string? CustomerAddress { get; set; }

    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
    [Display(Name = "Created")]
    public DateTime CreatedAt { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [StringLength(1000, MinimumLength = 10, ErrorMessage = "The description must be from 10 to 1000 characters.")]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Problem description")]
    public string? ProblemDescription { get; set; }

    [Range(typeof(decimal), "0", "1000000", ErrorMessage = "The cost must be from 0 to 1 000 000 UAH.")]
    [Column(TypeName = "decimal(10, 2)")]
    [DisplayFormat(DataFormatString = "{0:0.00} UAH")]
    [Display(Name = "Total cost")]
    public decimal? TotalCost { get; set; }

    [Display(Name = "Technician")]
    public int? TechnicianId { get; set; }
    public Technician? Technician { get; set; }

    // Laboratory work 6: the account that created the request
    // (null for old requests and for requests created by the administrator)
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    // Positions of the request (one-to-many)
    public List<OrderItem> Items { get; set; } = new();

    // History of status changes (one-to-many)
    public List<OrderStatusHistory> StatusHistory { get; set; } = new();
}
