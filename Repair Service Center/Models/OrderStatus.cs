using System.ComponentModel.DataAnnotations;

namespace Repair_Service_Center.Models;

/// <summary>
/// Life cycle of a repair request.
/// </summary>
public enum OrderStatus
{
    Pending = 0,
    Quoted = 1,
    Accepted = 2,
    [Display(Name = "In Progress")]
    InProgress = 3,
    Ready = 4,
    Issued = 5
}
