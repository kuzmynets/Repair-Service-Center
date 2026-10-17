using Microsoft.AspNetCore.Mvc.Localization;

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
    /// Laboratory work 8: with a localizer the units are translated ("1 год 30 хв").
    /// </summary>
    public static string FormatDuration(int minutes, IHtmlLocalizer? localizer = null)
    {
        string T(string format, params object[] args) =>
            localizer == null ? string.Format(format, args) : localizer.GetString(format, args).Value;

        if (minutes < 60) return T("{0} min", minutes);
        var hours = minutes / 60;
        var rest = minutes % 60;
        return rest == 0 ? T("{0} h", hours) : T("{0} h {1} min", hours, rest);
    }

    /// <summary>
    /// 4 -> "★★★★☆"
    /// </summary>
    public static string Stars(int rating)
    {
        rating = Math.Clamp(rating, 0, 5);
        return new string('\u2605', rating) + new string('\u2606', 5 - rating);
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
