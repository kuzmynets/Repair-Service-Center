namespace Repair_Service_Center.Helpers;

/// <summary>
/// Small helper methods for the user interface.
/// </summary>
public static class UiHelper
{
    /// <summary>
    /// Returns the file name of the icon for a device type (images/devices/...).
    /// </summary>
    public static string DeviceIcon(string? deviceName)
    {
        var name = (deviceName ?? string.Empty).ToLowerInvariant();

        if (name.Contains("phone")) return "smartphone.svg";
        if (name.Contains("laptop") || name.Contains("computer")) return "laptop.svg";
        if (name.Contains("tablet")) return "tablet.svg";
        if (name.Contains("washing")) return "washing-machine.svg";
        if (name.Contains("fridge") || name.Contains("refrigerator")) return "refrigerator.svg";
        return "device.svg";
    }

    /// <summary>
    /// 45 -> "45 min", 60 -> "1 h", 90 -> "1 h 30 min".
    /// </summary>
    public static string FormatDuration(int minutes)
    {
        if (minutes < 60) return $"{minutes} min";
        var hours = minutes / 60;
        var rest = minutes % 60;
        return rest == 0 ? $"{hours} h" : $"{hours} h {rest} min";
    }

    /// <summary>
    /// Bootstrap color class for the order status badge.
    /// </summary>
    public static string StatusBadge(Models.OrderStatus status) => status switch
    {
        Models.OrderStatus.Pending => "bg-secondary",
        Models.OrderStatus.Quoted => "bg-info text-dark",
        Models.OrderStatus.Accepted => "bg-primary",
        Models.OrderStatus.InProgress => "bg-warning text-dark",
        Models.OrderStatus.Ready => "bg-success",
        _ => "bg-dark"
    };
}
