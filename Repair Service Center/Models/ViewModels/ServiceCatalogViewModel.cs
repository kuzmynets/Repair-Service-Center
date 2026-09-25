namespace Repair_Service_Center.Models.ViewModels;

/// <summary>
/// Data for the service catalog page: services, filter and sort order.
/// </summary>
public class ServiceCatalogViewModel
{
    public List<PriceListItem> Services { get; set; } = new();
    public List<DeviceType> DeviceTypes { get; set; } = new();
    public int? SelectedDeviceTypeId { get; set; }
    public string Sort { get; set; } = "price";
}
