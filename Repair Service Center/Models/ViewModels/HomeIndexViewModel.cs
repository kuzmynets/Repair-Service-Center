namespace Repair_Service_Center.Models.ViewModels;

/// <summary>
/// Data for the home page.
/// </summary>
public class HomeIndexViewModel
{
    public List<DeviceType> DeviceTypes { get; set; } = new();
    public List<PriceListItem> FeaturedServices { get; set; } = new();
}
