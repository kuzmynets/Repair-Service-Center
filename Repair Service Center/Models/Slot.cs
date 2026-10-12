using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Repair_Service_Center.Validation;

namespace Repair_Service_Center.Models;

/// <summary>
/// Time slot in the schedule of a technician. A customer books a free slot for a repair.
/// </summary>
public class Slot : IValidatableObject
{
    public int Id { get; set; }

    [Display(Name = "Technician")]
    public int TechnicianId { get; set; }
    public Technician? Technician { get; set; }

    // Laboratory work 7: a new slot can be only in the future (own attribute, also checked in the browser)
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
    [FutureDate(ErrorMessage = "The start of a slot must be in the future.")]
    [Display(Name = "Start")]
    public DateTime StartTime { get; set; }

    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
    [Display(Name = "End")]
    public DateTime EndTime { get; set; }

    [Display(Name = "Booked")]
    public bool IsBooked { get; set; }

    /// <summary>
    /// Text like "Mon 06.10, 10:00-12:00" for drop-down lists. Not saved in the database.
    /// </summary>
    [NotMapped]
    public string TimeText => $"{StartTime:ddd dd.MM, HH:mm}\u2013{EndTime:HH:mm}";

    // Working day of the service center (laboratory work 7)
    private static readonly TimeSpan DayStart = new(8, 0, 0);
    private static readonly TimeSpan DayEnd = new(20, 0, 0);

    // Validation that uses several fields at once. It runs after the attributes of the properties.
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndTime <= StartTime)
        {
            yield return new ValidationResult("End time must be later than start time.", new[] { nameof(EndTime) });
            yield break;
        }

        if (StartTime.DayOfWeek == DayOfWeek.Sunday)
        {
            yield return new ValidationResult("Sunday is a day off.", new[] { nameof(StartTime) });
        }

        if (StartTime.Date != EndTime.Date)
        {
            yield return new ValidationResult("A slot must start and end on the same day.", new[] { nameof(EndTime) });
        }
        else if (StartTime.TimeOfDay < DayStart || EndTime.TimeOfDay > DayEnd)
        {
            yield return new ValidationResult("Working hours are from 08:00 to 20:00.", new[] { nameof(StartTime) });
        }

        var length = EndTime - StartTime;
        if (length < TimeSpan.FromMinutes(30) || length > TimeSpan.FromHours(4))
        {
            yield return new ValidationResult("A slot must last from 30 minutes to 4 hours.", new[] { nameof(EndTime) });
        }

        if (StartTime.Minute % 15 != 0 || EndTime.Minute % 15 != 0)
        {
            yield return new ValidationResult("Use times that are multiples of 15 minutes (10:00, 10:15, 10:30, ...).",
                new[] { nameof(StartTime) });
        }
    }
}
