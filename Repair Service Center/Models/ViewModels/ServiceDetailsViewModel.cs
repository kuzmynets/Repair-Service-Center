namespace Repair_Service_Center.Models.ViewModels;

/// <summary>
/// Data for the service details page.
/// </summary>
public class ServiceDetailsViewModel
{
    public PriceListItem Service { get; set; } = null!;
    public List<PriceListItem> RelatedServices { get; set; } = new();
}
