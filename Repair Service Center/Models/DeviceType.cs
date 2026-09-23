using System.ComponentModel.DataAnnotations;

namespace Repair_Service_Center.Models;

/// <summary>
/// Type of device that the service center repairs (smartphone, laptop, etc.).
/// </summary>
public class DeviceType
{
    public int Id { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 2)]
    [Display(Name = "Device type")]
    public string Name { get; set; } = string.Empty;
}
