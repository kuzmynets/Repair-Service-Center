using System.ComponentModel.DataAnnotations;

namespace Repair_Service_Center.Models.ViewModels;

/// <summary>
/// Registration form. Every new user gets the Customer role.
/// </summary>
public class RegisterViewModel
{
    [Required(ErrorMessage = "Please enter your full name.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "The name must be from 3 to 100 characters.")]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your e-mail.")]
    [EmailAddress(ErrorMessage = "Please enter a correct e-mail.")]
    [StringLength(100)]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your phone number.")]
    [Phone(ErrorMessage = "Please enter a correct phone number.")]
    [StringLength(20)]
    [Display(Name = "Phone")]
    public string PhoneNumber { get; set; } = string.Empty;

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
    [EmailAddress(ErrorMessage = "Please enter a correct e-mail.")]
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

    [Required(ErrorMessage = "Please enter your full name.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "The name must be from 3 to 100 characters.")]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your phone number.")]
    [Phone(ErrorMessage = "Please enter a correct phone number.")]
    [StringLength(20)]
    [Display(Name = "Phone")]
    public string PhoneNumber { get; set; } = string.Empty;
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
