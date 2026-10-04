using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
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

    // Validation that uses two fields at once
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndTime <= StartTime)
        {
            yield return new ValidationResult("End time must be later than start time.", new[] { nameof(EndTime) });
        }
    }
}
