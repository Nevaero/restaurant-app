using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RestaurantApp.Core.Entities;

namespace RestaurantApp.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Shift> Shifts => Set<Shift>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // SQLite has no native date/time-only types; map them to TEXT via converters.
        var dateOnlyConverter = new DateOnlyConverter();
        var timeOnlyConverter = new TimeOnlyConverter();

        modelBuilder.Entity<Menu>(b =>
        {
            b.Property(m => m.Name).IsRequired().HasMaxLength(120);
            b.Property(m => m.Date).HasConversion(dateOnlyConverter);
            b.HasMany(m => m.MenuItems)
                .WithOne(i => i.Menu)
                .HasForeignKey(i => i.MenuId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Ingredient>(b =>
        {
            b.Property(i => i.Name).IsRequired().HasMaxLength(120);
            b.Property(i => i.Unit).IsRequired().HasMaxLength(20);
            b.Property(i => i.StockQuantity).HasColumnType("decimal(18,3)");
            b.Property(i => i.LowStockThreshold).HasColumnType("decimal(18,3)");
        });

        modelBuilder.Entity<MenuItem>(b =>
        {
            b.Property(i => i.Dish).IsRequired().HasMaxLength(120);
            b.Property(i => i.QuantityRequired).HasColumnType("decimal(18,3)");
            b.HasOne(i => i.Ingredient)
                .WithMany(ing => ing.MenuItems)
                .HasForeignKey(i => i.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Employee>(b =>
        {
            b.Property(e => e.FirstName).IsRequired().HasMaxLength(80);
            b.Property(e => e.LastName).IsRequired().HasMaxLength(80);
            b.Property(e => e.Role).IsRequired().HasMaxLength(40);
        });

        modelBuilder.Entity<Shift>(b =>
        {
            b.Property(s => s.Date).HasConversion(dateOnlyConverter);
            b.Property(s => s.StartTime).HasConversion(timeOnlyConverter);
            b.Property(s => s.EndTime).HasConversion(timeOnlyConverter);
            b.HasOne(s => s.Employee)
                .WithMany(e => e.Shifts)
                .HasForeignKey(s => s.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

/// <summary>Stores <see cref="DateOnly"/> as ISO "yyyy-MM-dd" text (sortable in SQLite).</summary>
public sealed class DateOnlyConverter() : ValueConverter<DateOnly, string>(
    d => d.ToString("yyyy-MM-dd"),
    s => DateOnly.ParseExact(s, "yyyy-MM-dd"));

/// <summary>Stores <see cref="TimeOnly"/> as "HH:mm:ss" text.</summary>
public sealed class TimeOnlyConverter() : ValueConverter<TimeOnly, string>(
    t => t.ToString("HH:mm:ss"),
    s => TimeOnly.ParseExact(s, "HH:mm:ss"));
