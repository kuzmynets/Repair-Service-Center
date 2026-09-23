using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repair_Service_Center.Models;

/// <summary>
/// Price list entry: device type + repair type + complexity level -> price and duration.
/// </summary>
public class PriceListItem
{
    public int Id { get; set; }

    [Display(Name = "Device type")]
    public int DeviceTypeId { get; set; }
    public DeviceType? DeviceType { get; set; }

    [Display(Name = "Repair type")]
    public int RepairTypeId { get; set; }
    public RepairType? RepairType { get; set; }

    [Display(Name = "Complexity level")]
    public int ComplexityLevelId { get; set; }
    public ComplexityLevel? ComplexityLevel { get; set; }

    [Range(typeof(decimal), "1", "100000")]
    [Column(TypeName = "decimal(10, 2)")]
    [DisplayFormat(DataFormatString = "{0:0.00} UAH")]
    [Display(Name = "Price")]
    public decimal Price { get; set; }

    [Range(5, 10080)]
    [Display(Name = "Duration (min)")]
    public int DurationMinutes { get; set; }
}
