using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repair_Service_Center.Models;

/// <summary>
/// Repair request from a customer.
/// </summary>
public class Order
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    [Display(Name = "Customer")]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [Phone]
    [StringLength(20)]
    [Display(Name = "Phone")]
    public string CustomerPhone { get; set; } = string.Empty;

    // ----- Laboratory work 5: new fields -----
    [EmailAddress]
    [StringLength(100)]
    [Display(Name = "Email")]
    public string? CustomerEmail { get; set; }

    [StringLength(50)]
    [Display(Name = "Brand")]
    public string? DeviceBrand { get; set; }

    [StringLength(50)]
    [Display(Name = "Model")]
    public string? DeviceModel { get; set; }

    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
    [Display(Name = "Created")]
    public DateTime CreatedAt { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [StringLength(1000)]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Problem description")]
    public string? ProblemDescription { get; set; }

    [Range(typeof(decimal), "0", "1000000")]
    [Column(TypeName = "decimal(10, 2)")]
    [DisplayFormat(DataFormatString = "{0:0.00} UAH")]
    [Display(Name = "Total cost")]
    public decimal? TotalCost { get; set; }

    [Display(Name = "Technician")]
    public int? TechnicianId { get; set; }
    public Technician? Technician { get; set; }

    // Positions of the request (one-to-many)
    public List<OrderItem> Items { get; set; } = new();

    // History of status changes (one-to-many)
    public List<OrderStatusHistory> StatusHistory { get; set; } = new();
}
