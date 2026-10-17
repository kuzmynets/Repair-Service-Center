using System.ComponentModel.DataAnnotations;

namespace Repair_Service_Center.Models;

/// <summary>
/// Life cycle of a repair request.
/// </summary>
// Laboratory work 8: every value has [Display]. Html.GetEnumSelectList<OrderStatus>() translates
// only the names from [Display], and views translate GetDisplayName() with SharedLocalizer.
public enum OrderStatus
{
    [Display(Name = "Pending")]
    Pending = 0,
    [Display(Name = "Quoted")]
    Quoted = 1,
    [Display(Name = "Accepted")]
    Accepted = 2,
    [Display(Name = "In Progress")]
    InProgress = 3,
    [Display(Name = "Ready")]
    Ready = 4,
    [Display(Name = "Issued")]
    Issued = 5
}
