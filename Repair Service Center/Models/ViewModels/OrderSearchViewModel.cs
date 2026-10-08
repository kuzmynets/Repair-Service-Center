using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Repair_Service_Center.Models.ViewModels;

/// <summary>
/// Administrator: list of orders and the search criteria.
/// </summary>
public class OrderSearchViewModel
{
    public List<Order> Orders { get; set; } = new();

    public SelectList? Technicians { get; set; }

    [Display(Name = "Search")]
    public string? SearchString { get; set; }

    [Display(Name = "Status")]
    public OrderStatus? Status { get; set; }

    [Display(Name = "Technician")]
    public int? TechnicianId { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "From")]
    public DateTime? DateFrom { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "To")]
    public DateTime? DateTo { get; set; }
}
