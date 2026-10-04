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

    public OrderStatus Status { get; set; }

    [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
    [Display(Name = "Changed")]
    public DateTime ChangedAt { get; set; }

    [StringLength(200)]
    public string? Comment { get; set; }
}
