using Microsoft.EntityFrameworkCore;
using Repair_Service_Center.Models;

namespace Repair_Service_Center.Data;

/// <summary>
/// Fills the database with data that depends on the current date.
/// Static data (directories, price list) is added by HasData in the migration.
/// </summary>
public static class SeedData
{
    // Start hours of the slots; every slot lasts 2 hours
    private static readonly int[] WorkHours = { 10, 12, 14, 16 };

    /// <summary>
    /// Called once at application start (see Program.cs).
    /// </summary>
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = new RepairServiceContext(
            serviceProvider.GetRequiredService<DbContextOptions<RepairServiceContext>>());

        // If there are future slots, the schedule is already filled
        if (context.Slots.Any(s => s.StartTime > DateTime.Now))
        {
            return;
        }

        AddSlots(context, DateTime.Today.AddDays(1), 7);
    }

    /// <summary>
    /// Adds 2-hour slots for every technician for the given number of days
    /// (Sunday is a day off). Existing slots are not duplicated.
    /// Returns the number of added slots.
    /// </summary>
    public static int AddSlots(RepairServiceContext context, DateTime fromDate, int days)
    {
        var technicianIds = context.Technicians.Select(t => t.Id).ToList();

        var existing = context.Slots
            .Where(s => s.StartTime >= fromDate.Date)
            .Select(s => new { s.TechnicianId, s.StartTime })
            .AsEnumerable()
            .Select(s => (s.TechnicianId, s.StartTime))
            .ToHashSet();

        var added = 0;
        for (var d = 0; d < days; d++)
        {
            var day = fromDate.Date.AddDays(d);
            if (day.DayOfWeek == DayOfWeek.Sunday)
            {
                continue;
            }

            foreach (var technicianId in technicianIds)
            {
                foreach (var hour in WorkHours)
                {
                    var start = day.AddHours(hour);
                    if (existing.Contains((technicianId, start)))
                    {
                        continue;
                    }

                    context.Slots.Add(new Slot
                    {
                        TechnicianId = technicianId,
                        StartTime = start,
                        EndTime = start.AddHours(2)
                    });
                    added++;
                }
            }
        }

        context.SaveChanges();
        return added;
    }
}
