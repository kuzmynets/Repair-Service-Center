namespace Repair_Service_Center.Models;

/// <summary>
/// Names of the roles. They are kept in one place, so there are no typos in [Authorize].
/// A guest (unauthorized user) has no role: he is simply not logged in.
/// </summary>
public static class AppRoles
{
    public const string Admin = "Admin";           // resource administrator
    public const string Moderator = "Moderator";   // moderator of reviews and comments
    public const string Customer = "Customer";     // authorized user

    public static readonly string[] All = { Admin, Moderator, Customer };
}

/// <summary>
/// Names of the authorization policies (they are registered in Program.cs).
/// </summary>
public static class AppPolicies
{
    public const string RequireAdmin = "RequireAdmin";
    public const string CanModerateReviews = "CanModerateReviews";
}
