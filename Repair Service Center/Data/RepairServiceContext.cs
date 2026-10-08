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
    public DbSet<Slot> Slots => Set<Slot>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OrderStatusHistory> OrderStatusHistory => Set<OrderStatusHistory>();
    public DbSet<Review> Reviews => Set<Review>();

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

        // ----- Laboratory work 4: schedule, order items, status history, reviews -----

        // Slots of a technician are deleted together with the technician
        modelBuilder.Entity<Slot>()
            .HasOne(s => s.Technician).WithMany()
            .HasForeignKey(s => s.TechnicianId)
            .OnDelete(DeleteBehavior.Cascade);

        // A technician cannot have two slots that start at the same time
        modelBuilder.Entity<Slot>()
            .HasIndex(s => new { s.TechnicianId, s.StartTime })
            .IsUnique();

        // Order items are deleted together with the order
        modelBuilder.Entity<OrderItem>()
            .HasOne(i => i.Order).WithMany(o => o.Items)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // A service from the price list cannot be deleted while it is used in orders
        modelBuilder.Entity<OrderItem>()
            .HasOne(i => i.PriceListItem).WithMany()
            .HasForeignKey(i => i.PriceListItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // A booked slot cannot be deleted
        modelBuilder.Entity<OrderItem>()
            .HasOne(i => i.Slot).WithMany()
            .HasForeignKey(i => i.SlotId)
            .OnDelete(DeleteBehavior.Restrict);

        // One slot can be booked only by one order item (protection from double booking)
        modelBuilder.Entity<OrderItem>()
            .HasIndex(i => i.SlotId)
            .IsUnique()
            .HasFilter("[SlotId] IS NOT NULL");

        modelBuilder.Entity<OrderStatusHistory>()
            .HasOne(h => h.Order).WithMany(o => o.StatusHistory)
            .HasForeignKey(h => h.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderStatusHistory>()
            .Property(h => h.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        // If an order is deleted, the review stays without the order number
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Order).WithMany()
            .HasForeignKey(r => r.OrderId)
            .OnDelete(DeleteBehavior.SetNull);

        AddInitialData(modelBuilder);
    }

    /// <summary>
    /// Initial data that is added to the database by the migration.
    /// </summary>
    private static void AddInitialData(ModelBuilder modelBuilder)
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
            new Technician { Id = 1, FullName = "Oleksandr Kovalenko", Specialization = "Smartphones and tablets", Phone = "+380671234567", ExperienceYears = 7 },
            new Technician { Id = 2, FullName = "Iryna Melnyk", Specialization = "Laptops and computers", Phone = "+380502345678", ExperienceYears = 5 },
            new Technician { Id = 3, FullName = "Petro Bondarenko", Specialization = "Home appliances", Phone = "+380933456789", ExperienceYears = 10 });

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
                CustomerEmail = "anna.shevchenko@example.com", DeviceBrand = "Samsung", DeviceModel = "Galaxy A54",
                CreatedAt = new DateTime(2026, 9, 20, 10, 30, 0), Status = OrderStatus.Accepted,
                ProblemDescription = "Broken screen after a fall", TotalCost = 1500m, TechnicianId = 1
            },
            new Order
            {
                Id = 2, CustomerName = "Dmytro Tkachenko", CustomerPhone = "+380672223344",
                DeviceBrand = "Lenovo", DeviceModel = "IdeaPad 5",
                CreatedAt = new DateTime(2026, 9, 22, 14, 15, 0), Status = OrderStatus.Pending,
                ProblemDescription = "Laptop turns off after 10 minutes of work"
            });

        // Items and status history of the two demo orders
        modelBuilder.Entity<OrderItem>().HasData(
            new OrderItem { Id = 1, OrderId = 1, PriceListItemId = 2, Price = 1500m, DurationMinutes = 60 });

        modelBuilder.Entity<OrderStatusHistory>().HasData(
            new OrderStatusHistory { Id = 1, OrderId = 1, Status = OrderStatus.Accepted, ChangedAt = new DateTime(2026, 9, 20, 10, 30, 0), Comment = "Request created" },
            new OrderStatusHistory { Id = 2, OrderId = 2, Status = OrderStatus.Pending, ChangedAt = new DateTime(2026, 9, 22, 14, 15, 0), Comment = "Request created" });

        modelBuilder.Entity<Review>().HasData(
            new Review { Id = 1, AuthorName = "Anna Shevchenko", Rating = 5, Text = "The screen of my phone was replaced in one hour. Thank you!", OrderId = 1, CreatedAt = new DateTime(2026, 9, 21), IsApproved = true },
            new Review { Id = 2, AuthorName = "Ihor Petrenko", Rating = 4, Text = "Good diagnostics of my washing machine and a clear price.", CreatedAt = new DateTime(2026, 9, 15), IsApproved = true },
            new Review { Id = 3, AuthorName = "Olena Kravets", Rating = 5, Text = "Fast laptop cleaning, now it is quiet again.", CreatedAt = new DateTime(2026, 9, 18), IsApproved = true },
            new Review { Id = 4, AuthorName = "Taras", Rating = 3, Text = "Still waiting for an answer about my laptop.", CreatedAt = new DateTime(2026, 9, 25), IsApproved = false });
    }
}
