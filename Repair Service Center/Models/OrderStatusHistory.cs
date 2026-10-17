using System.ComponentModel.DataAnnotations;

namespace Repair_Service_Center.Models;

/// <summary>
/// One record about a change of the order status.
/// </summary>
public class OrderStatusHistory
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order? Order { get; set; }

    [Display(Name = "Status")]
    public OrderStatus Status { get; set; }

    [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
    [Display(Name = "Changed")]
    public DateTime ChangedAt { get; set; }

    [StringLength(200, ErrorMessage = "The field {0} must be at most {1} characters long.")]
    [Display(Name = "Comment")]
    public string? Comment { get; set; }
}
