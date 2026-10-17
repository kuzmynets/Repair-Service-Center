using System.ComponentModel.DataAnnotations;
using Repair_Service_Center.Validation;

namespace Repair_Service_Center.Models;

/// <summary>
/// Customer review. It is shown on the site only after moderation.
/// </summary>
public class Review
{
    public int Id { get; set; }

    // Laboratory work 7: the author - one to three words with capital letters
    [Required(ErrorMessage = "Please enter your name.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "The field {0} must be from {2} to {1} characters long.")]
    [PersonName(1, 3)]
    [Display(Name = "Your name")]
    public string AuthorName { get; set; } = string.Empty;

    [Required(ErrorMessage = "The field {0} is required.")]
    [Range(1, 5, ErrorMessage = "Rating must be from 1 to 5.")]
    [Display(Name = "Rating")]
    public int Rating { get; set; } = 5;

    [Required(ErrorMessage = "Please write a few words.")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "The review must be from 10 to 1000 characters.")]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Review")]
    public string Text { get; set; } = string.Empty;

    // Number of the repair request (optional)
    [Display(Name = "Request number")]
    public int? OrderId { get; set; }
    public Order? Order { get; set; }

    [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
    [Display(Name = "Date")]
    public DateTime CreatedAt { get; set; }

    [Display(Name = "Approved")]
    public bool IsApproved { get; set; }
}
