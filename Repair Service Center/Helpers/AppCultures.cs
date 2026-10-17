using System.Globalization;

namespace Repair_Service_Center.Helpers;

/// <summary>
/// Laboratory work 8. Languages of the site: English (default) and Ukrainian.
/// </summary>
public static class AppCultures
{
    public const string English = "en-US";
    public const string Ukrainian = "uk-UA";

    public static readonly CultureInfo[] Supported = { Create(English), Create(Ukrainian) };

    public static bool IsSupported(string? name) =>
        Supported.Any(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));

    // In Ukrainian the decimal separator is a comma (12,50), but <input type="number">
    // and jQuery Validation always send a dot (12.50). The dot is kept for both languages,
    // so the price is read correctly in any language. Dates, day and month names follow the language.
    private static CultureInfo Create(string name)
    {
        var culture = new CultureInfo(name);
        culture.NumberFormat.NumberDecimalSeparator = ".";
        culture.NumberFormat.CurrencyDecimalSeparator = ".";
        return culture;
    }
}
