using Microsoft.Extensions.Localization;

// Laboratory work 8. The name of the assembly ("Repair Service Center") has spaces,
// so it is different from the root namespace (Repair_Service_Center).
// Localization builds the names of the resources from the root namespace,
// so we tell it the real root namespace (see the Microsoft documentation, "RootNamespaceAttribute").
[assembly: RootNamespace("Repair_Service_Center")]

namespace Repair_Service_Center;

/// <summary>
/// Empty marker class for the shared resources of the site.
/// IStringLocalizer&lt;SharedResource&gt; and IHtmlLocalizer&lt;SharedResource&gt; read
/// the file Resources/SharedResource.uk.resx. The English text is the key,
/// so English needs no .resx file: if a translation is not found, the key itself is shown.
/// </summary>
public class SharedResource
{
}
