using Microsoft.EntityFrameworkCore;
using Repair_Service_Center.Models;

namespace Repair_Service_Center.Data;

/// <summary>
/// Database context of the application. Each DbSet is a table in the database.
/// </summary>
public class RepairServiceContext : DbContext
{
    public RepairServiceContext(DbContextOptions<RepairServiceContext> options)
        : base(options)
    {
    }

    public DbSet<DeviceType> DeviceTypes => Set<DeviceType>();
    public DbSet<RepairType> RepairTypes => Set<RepairType>();
    public DbSet<ComplexityLevel> ComplexityLevels => Set<ComplexityLevel>();
    public DbSet<Technician> Technicians => Set<Technician>();
    public DbSet<PriceListItem> PriceListItems => Set<PriceListItem>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // A directory entry cannot be deleted while it is used in the price list
        modelBuilder.Entity<PriceListItem>()
            .HasOne(p => p.DeviceType).WithMany()
            .HasForeignKey(p => p.DeviceTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PriceListItem>()
            .HasOne(p => p.RepairType).WithMany()
            .HasForeignKey(p => p.RepairTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PriceListItem>()
            .HasOne(p => p.ComplexityLevel).WithMany()
            .HasForeignKey(p => p.ComplexityLevelId)
            .OnDelete(DeleteBehavior.Restrict);

        // If a technician is deleted, his orders stay without a technician
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Technician).WithMany()
            .HasForeignKey(o => o.TechnicianId)
            .OnDelete(DeleteBehavior.SetNull);

        // Status is stored as text, so the database is easier to read
        modelBuilder.Entity<Order>()
            .Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        SeedData(modelBuilder);
    }

    /// <summary>
    /// Initial data that is added to the database by the migration.
    /// </summary>
    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DeviceType>().HasData(
            new DeviceType { Id = 1, Name = "Smartphone" },
            new DeviceType { Id = 2, Name = "Laptop" },
            new DeviceType { Id = 3, Name = "Tablet" },
            new DeviceType { Id = 4, Name = "Washing machine" },
            new DeviceType { Id = 5, Name = "Refrigerator" });

        modelBuilder.Entity<RepairType>().HasData(
            new RepairType { Id = 1, Name = "Diagnostics", Description = "Full check of the device to find the problem" },
            new RepairType { Id = 2, Name = "Screen replacement", Description = "Replacement of a broken display module" },
            new RepairType { Id = 3, Name = "Battery replacement", Description = "Replacement of an old or swollen battery" },
            new RepairType { Id = 4, Name = "Cleaning", Description = "Dust cleaning and thermal paste replacement" },
            new RepairType { Id = 5, Name = "Software reinstall", Description = "Reinstall of the operating system and drivers" });

        modelBuilder.Entity<ComplexityLevel>().HasData(
            new ComplexityLevel { Id = 1, Name = "Budget" },
            new ComplexityLevel { Id = 2, Name = "Mid-range" },
            new ComplexityLevel { Id = 3, Name = "Premium" });

        modelBuilder.Entity<Technician>().HasData(
            new Technician { Id = 1, FullName = "Oleksandr Kovalenko", Specialization = "Smartphones and tablets", Phone = "+380671234567" },
            new Technician { Id = 2, FullName = "Iryna Melnyk", Specialization = "Laptops and computers", Phone = "+380502345678" },
            new Technician { Id = 3, FullName = "Petro Bondarenko", Specialization = "Home appliances", Phone = "+380933456789" });

        modelBuilder.Entity<PriceListItem>().HasData(
            new PriceListItem { Id = 1, DeviceTypeId = 1, RepairTypeId = 1, ComplexityLevelId = 1, Price = 200m, DurationMinutes = 30 },
            new PriceListItem { Id = 2, DeviceTypeId = 1, RepairTypeId = 2, ComplexityLevelId = 1, Price = 1500m, DurationMinutes = 60 },
            new PriceListItem { Id = 3, DeviceTypeId = 1, RepairTypeId = 2, ComplexityLevelId = 3, Price = 4500m, DurationMinutes = 90 },
            new PriceListItem { Id = 4, DeviceTypeId = 1, RepairTypeId = 3, ComplexityLevelId = 2, Price = 900m, DurationMinutes = 45 },
            new PriceListItem { Id = 5, DeviceTypeId = 2, RepairTypeId = 4, ComplexityLevelId = 2, Price = 800m, DurationMinutes = 60 },
            new PriceListItem { Id = 6, DeviceTypeId = 2, RepairTypeId = 5, ComplexityLevelId = 1, Price = 500m, DurationMinutes = 60 },
            new PriceListItem { Id = 7, DeviceTypeId = 4, RepairTypeId = 1, ComplexityLevelId = 1, Price = 350m, DurationMinutes = 45 });

        modelBuilder.Entity<Order>().HasData(
            new Order
            {
                Id = 1, CustomerName = "Anna Shevchenko", CustomerPhone = "+380501112233",
                CreatedAt = new DateTime(2026, 9, 20, 10, 30, 0), Status = OrderStatus.Accepted,
                ProblemDescription = "Broken screen after a fall", TotalCost = 1500m, TechnicianId = 1
            },
            new Order
            {
                Id = 2, CustomerName = "Dmytro Tkachenko", CustomerPhone = "+380672223344",
                CreatedAt = new DateTime(2026, 9, 22, 14, 15, 0), Status = OrderStatus.Pending,
                ProblemDescription = "Laptop turns off after 10 minutes of work"
            });
    }
}
