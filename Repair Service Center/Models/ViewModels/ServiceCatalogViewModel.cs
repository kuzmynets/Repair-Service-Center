using Microsoft.AspNetCore.Mvc.Rendering;

namespace Repair_Service_Center.Models.ViewModels;

/// <summary>
/// Data for the service catalog: found services and the search criteria.
/// </summary>
public class ServiceCatalogViewModel
{
    public List<PriceListItem> Services { get; set; } = new();
    public List<DeviceType> DeviceTypes { get; set; } = new();

    // Lists for the drop-downs
    public SelectList? DeviceTypeList { get; set; }
    public SelectList? RepairTypeList { get; set; }
    public SelectList? ComplexityLevelList { get; set; }
    public SelectList? SortList { get; set; }

    // Search criteria (they come from the query string)
    public string? SearchString { get; set; }
    public int? SelectedDeviceTypeId { get; set; }
    public int? RepairTypeId { get; set; }
    public int? ComplexityLevelId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string Sort { get; set; } = "price";

    public bool HasFilters =>
        !string.IsNullOrWhiteSpace(SearchString) || SelectedDeviceTypeId.HasValue ||
        RepairTypeId.HasValue || ComplexityLevelId.HasValue || MinPrice.HasValue || MaxPrice.HasValue;
}
