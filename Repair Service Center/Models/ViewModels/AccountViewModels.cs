using System.ComponentModel.DataAnnotations;
using Repair_Service_Center.Validation;

namespace Repair_Service_Center.Models.ViewModels;

/// <summary>
/// Registration form. Every new user gets the Customer role.
/// </summary>
public class RegisterViewModel
{
    // Laboratory work 7: first name and last name are checked separately
    [Required(ErrorMessage = "Please enter your first name.")]
    [StringLength(50, MinimumLength = 2)]
    [PersonName]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your last name.")]
    [StringLength(50, MinimumLength = 2)]
    [PersonName]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your e-mail.")]
    [EmailFormat]
    [DataType(DataType.EmailAddress)]
    [StringLength(100)]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your mobile phone.")]
    [MobilePhone]
    [DataType(DataType.PhoneNumber)]
    [StringLength(20)]
    [Display(Name = "Mobile phone")]
    public string PhoneNumber { get; set; } = string.Empty;

    [StringLength(200)]
    [Address]
    [Display(Name = "Address (optional)")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "Please enter a password.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "The password must have at least 8 characters.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please repeat the password.")]
    [Compare(nameof(Password), ErrorMessage = "The passwords do not match.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// Login form.
/// </summary>
public class LoginViewModel
{
    [Required(ErrorMessage = "Please enter your e-mail.")]
    [EmailFormat]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your password.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }
}

/// <summary>
/// Profile of the logged-in user (the e-mail is the login, so it is not changed here).
/// </summary>
public class ProfileViewModel
{
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Role")]
    public string Role { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your first name.")]
    [StringLength(50, MinimumLength = 2)]
    [PersonName]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your last name.")]
    [StringLength(50, MinimumLength = 2)]
    [PersonName]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your mobile phone.")]
    [MobilePhone]
    [DataType(DataType.PhoneNumber)]
    [StringLength(20)]
    [Display(Name = "Mobile phone")]
    public string PhoneNumber { get; set; } = string.Empty;

    [StringLength(200)]
    [Address]
    [Display(Name = "Address (optional)")]
    public string? Address { get; set; }
}

/// <summary>
/// A row in the administrator's list of users.
/// </summary>
public class UserRoleViewModel
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Role { get; set; } = string.Empty;
    public int OrdersCount { get; set; }
    public bool IsCurrentUser { get; set; }
}
