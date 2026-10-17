using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repair_Service_Center.Models;

/// <summary>
/// One position (service) inside a repair request.
/// </summary>
public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order? Order { get; set; }

    // Null for a custom request that is not connected to the price list
    [Display(Name = "Service")]
    public int? PriceListItemId { get; set; }
    public PriceListItem? PriceListItem { get; set; }

    [Display(Name = "Time slot")]
    public int? SlotId { get; set; }
    public Slot? Slot { get; set; }

    // The price is copied at the moment of the order,
    // so later changes in the price list do not change old orders
    [Column(TypeName = "decimal(10, 2)")]
    // Laboratory work 8: only the number; the currency (UAH / грн) is added in the view in the language of the user
    [DisplayFormat(DataFormatString = "{0:0.00}")]
    [Display(Name = "Price")]
    public decimal Price { get; set; }

    [Display(Name = "Duration (min)")]
    public int DurationMinutes { get; set; }
}
