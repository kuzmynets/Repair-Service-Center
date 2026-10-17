using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Repair_Service_Center.Helpers;

/// <summary>
/// Laboratory work 8. ASP.NET Core Identity has its own English messages
/// ("Passwords must have at least one digit", "Email is already taken").
/// This class replaces them with texts from Resources/SharedResource.uk.resx,
/// so the registration form shows the errors in the language of the user.
/// It is registered in Program.cs with AddErrorDescriber&lt;LocalizedIdentityErrorDescriber&gt;().
/// </summary>
public class LocalizedIdentityErrorDescriber : IdentityErrorDescriber
{
    private readonly IStringLocalizer<SharedResource> _localizer;

    public LocalizedIdentityErrorDescriber(IStringLocalizer<SharedResource> localizer)
    {
        _localizer = localizer;
    }

    private IdentityError Error(string code, string text, params object[] args) =>
        new() { Code = code, Description = _localizer[text, args].Value };

    public override IdentityError DefaultError() =>
        Error(nameof(DefaultError), "An unknown error has occurred.");

    public override IdentityError ConcurrencyFailure() =>
        Error(nameof(ConcurrencyFailure), "The data was changed by another user. Please try again.");

    public override IdentityError PasswordMismatch() =>
        Error(nameof(PasswordMismatch), "Wrong password.");

    public override IdentityError InvalidUserName(string? userName) =>
        Error(nameof(InvalidUserName), "The login {0} is not valid.", userName ?? string.Empty);

    public override IdentityError InvalidEmail(string? email) =>
        Error(nameof(InvalidEmail), "The e-mail {0} is not valid.", email ?? string.Empty);

    public override IdentityError DuplicateUserName(string userName) =>
        Error(nameof(DuplicateUserName), "The login {0} is already taken.", userName);

    public override IdentityError DuplicateEmail(string email) =>
        Error(nameof(DuplicateEmail), "The e-mail {0} is already registered.", email);

    public override IdentityError PasswordTooShort(int length) =>
        Error(nameof(PasswordTooShort), "The password must have at least {0} characters.", length);

    public override IdentityError PasswordRequiresDigit() =>
        Error(nameof(PasswordRequiresDigit), "The password must have at least one digit (0-9).");

    public override IdentityError PasswordRequiresLower() =>
        Error(nameof(PasswordRequiresLower), "The password must have at least one small letter (a-z).");

    public override IdentityError PasswordRequiresUpper() =>
        Error(nameof(PasswordRequiresUpper), "The password must have at least one capital letter (A-Z).");

    public override IdentityError PasswordRequiresNonAlphanumeric() =>
        Error(nameof(PasswordRequiresNonAlphanumeric), "The password must have at least one special character.");
}
