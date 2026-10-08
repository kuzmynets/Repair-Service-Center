using System.ComponentModel.DataAnnotations;

namespace Repair_Service_Center.Models.ViewModels;

/// <summary>
/// "Track my request": search of an order by its number and the phone number.
/// </summary>
public class TrackRequestViewModel
{
    [Display(Name = "Request number")]
    public int? OrderId { get; set; }

    [Display(Name = "Phone number")]
    public string? Phone { get; set; }

    // Found order (null if nothing was found)
    public Order? Order { get; set; }

    // True after the user pressed "Find"
    public bool Searched { get; set; }
}
