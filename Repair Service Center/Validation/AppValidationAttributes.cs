using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace Repair_Service_Center.Validation;

/// <summary>
/// Laboratory work 7. Regular expressions that are used in several models.
/// They are written once (DRY principle) and used in the attributes below.
/// </summary>
public static class ValidationPatterns
{
    // Latin and Ukrainian letters
    public const string Letters = "A-Za-zА-Яа-яІіЇїЄєҐґ";
    public const string UpperLetters = "A-ZА-ЯІЇЄҐ";

    // Name of a directory record: starts with a capital letter ("Screen replacement", "Mid-range")
    public const string Title = "^[" + UpperLetters + "][" + Letters + "0-9 ’'(),.&\\-]*$";
    public const string TitleMessage = "Must start with a capital letter; only letters, digits, spaces and - ' ( ) , . & are allowed.";

    // Brand or model of a device: "Samsung", "Galaxy A54", "iPhone 15 Pro"
    public const string Device = "^[" + Letters + "0-9 .+\\-]*$";
    public const string DeviceMessage = "Only letters, digits, spaces and the characters . + - are allowed.";
}

/// <summary>
/// Laboratory work 8. Translation of validation messages with Resources/SharedResource.uk.resx.
/// Own attributes and IValidatableObject do not get the localizer from MVC,
/// so they take IStringLocalizerFactory from the services of the current request.
/// </summary>
public static class ValidationText
{
    public static string Translate(IServiceProvider? services, string text, params object[] args)
    {
        var factory = services?.GetService(typeof(IStringLocalizerFactory)) as IStringLocalizerFactory;
        if (factory == null)
        {
            return string.Format(CultureInfo.CurrentCulture, text, args);
        }
        return factory.Create(typeof(SharedResource))[text, args].Value;
    }
}

/// <summary>
/// Base class for the rules with a regular expression. The same rule works
/// on the server (IsValid of RegularExpressionAttribute) and in the browser:
/// AddValidation writes data-val-regex attributes, and jQuery Validation checks them.
/// </summary>
public abstract class PatternAttribute : RegularExpressionAttribute, IClientModelValidator
{
    protected PatternAttribute(string pattern, string message) : base(pattern)
    {
        ErrorMessage = message;
    }

    // Server: the message in the language of the user (laboratory work 8)
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (IsValid(value))
        {
            return ValidationResult.Success;
        }
        var message = ValidationText.Translate(validationContext, ErrorMessageString, validationContext.DisplayName);
        return new ValidationResult(message, validationContext.MemberName == null ? null : new[] { validationContext.MemberName });
    }

    // Browser: the same translated message goes to data-val-regex
    public void AddValidation(ClientModelValidationContext context)
    {
        var message = ValidationText.Translate(context.ActionContext.HttpContext.RequestServices,
            ErrorMessageString, context.ModelMetadata.GetDisplayName());
        Merge(context.Attributes, "data-val", "true");
        Merge(context.Attributes, "data-val-regex", message);
        Merge(context.Attributes, "data-val-regex-pattern", Pattern);
    }

    private static void Merge(IDictionary<string, string> attributes, string key, string value)
    {
        if (!attributes.ContainsKey(key))
        {
            attributes.Add(key, value);
        }
    }
}

/// <summary>
/// First name, last name, full name or author: every word starts with a capital letter,
/// only letters, apostrophe and hyphen (O'Neil, Ivanenko-Petrenko, Ольга).
/// [PersonName] - one word; [PersonName(2, 4)] - full name of 2-4 words.
/// </summary>
public class PersonNameAttribute : PatternAttribute
{
    private const string Word = "[" + ValidationPatterns.UpperLetters + "][" + ValidationPatterns.Letters + "'’\\-]*[" + ValidationPatterns.Letters + "]";

    public PersonNameAttribute(int minWords = 1, int maxWords = 1)
        : base(BuildPattern(minWords, maxWords), BuildMessage(minWords, maxWords))
    {
    }

    private static string BuildPattern(int minWords, int maxWords) =>
        maxWords <= 1
            ? "^" + Word + "$"
            : "^" + Word + "( " + Word + "){" + (minWords - 1) + "," + (maxWords - 1) + "}$";

    private static string BuildMessage(int minWords, int maxWords) =>
        maxWords <= 1
            ? "Use only letters, apostrophe and hyphen; start with a capital letter (e.g. Anna)."
            : minWords >= 2
                ? "Enter the first and the last name with capital letters (e.g. Anna Shevchenko)."
                : "Use only letters; every word starts with a capital letter (e.g. Anna or Anna Shevchenko).";
}

/// <summary>
/// Ukrainian mobile number in the international format +380XXXXXXXXX
/// with a code of a mobile operator (050, 063, 066, 067, 068, 073, 091-099, 039).
/// </summary>
public class MobilePhoneAttribute : PatternAttribute
{
    public MobilePhoneAttribute()
        : base(@"^\+380(39|50|63|66|67|68|73|9[1-9])\d{7}$",
            "Enter a Ukrainian mobile number in the format +380XXXXXXXXX (e.g. +380501234567).")
    {
    }
}

/// <summary>
/// E-mail address. The standard [EmailAddress] only checks that there is "@",
/// this rule also needs a domain with a dot: name@example.com.
/// </summary>
public class EmailFormatAttribute : PatternAttribute
{
    public EmailFormatAttribute()
        : base(@"^[A-Za-z0-9._%+\-]+@[A-Za-z0-9.\-]+\.[A-Za-z]{2,}$",
            "Enter a correct e-mail (e.g. name@example.com).")
    {
    }
}

/// <summary>
/// Postal address: 10-200 characters, letters, digits, spaces and . , ' / -;
/// it must have letters (city, street) and at least one digit (house number).
/// </summary>
public class AddressAttribute : PatternAttribute
{
    public AddressAttribute()
        : base("^(?=.*[" + ValidationPatterns.Letters + "])(?=.*\\d)[" + ValidationPatterns.Letters + "0-9 .,'’/\\-]{10,200}$",
            "Enter the city, the street and the house number (e.g. Khmelnytskyi, Instytutska St, 11).")
    {
    }
}

/// <summary>
/// Date and time must be in the future. This rule has no standard client adapter,
/// so wwwroot/js/validation.js adds the method "futuredate" to jQuery Validation.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class FutureDateAttribute : ValidationAttribute, IClientModelValidator
{
    public FutureDateAttribute() : base("The date and time must be in the future.")
    {
    }

    // An empty value is valid: [Required] checks it
    public override bool IsValid(object? value) => value is not DateTime date || date > DateTime.Now;

    // Laboratory work 8: translated message on the server ...
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (IsValid(value))
        {
            return ValidationResult.Success;
        }
        var message = ValidationText.Translate(validationContext, ErrorMessageString, validationContext.DisplayName);
        return new ValidationResult(message, validationContext.MemberName == null ? null : new[] { validationContext.MemberName });
    }

    // ... and in the browser
    public void AddValidation(ClientModelValidationContext context)
    {
        if (!context.Attributes.ContainsKey("data-val"))
        {
            context.Attributes.Add("data-val", "true");
        }
        context.Attributes["data-val-futuredate"] = ValidationText.Translate(
            context.ActionContext.HttpContext.RequestServices, ErrorMessageString, context.ModelMetadata.GetDisplayName());
    }
}
