using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Repair_Service_Center.Models;

public static class EnumExtensions
{
    /// <summary>
    /// Returns the name from the [Display] attribute, or the enum value name.
    /// </summary>
    public static string GetDisplayName(this Enum value)
    {
        return value.GetType()
                   .GetMember(value.ToString())
                   .FirstOrDefault()?
                   .GetCustomAttribute<DisplayAttribute>()?
                   .GetName()
               ?? value.ToString();
    }
}
