using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RestaurantApp.Core.Entities;

namespace RestaurantApp.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<MenuDay> MenuDays => Set<MenuDay>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<Allergen> Allergens => Set<Allergen>();
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
            b.Property(m => m.WeekStart).HasConversion(dateOnlyConverter);
            b.Property(m => m.Content).HasMaxLength(8000);
        });

        modelBuilder.Entity<MenuDay>(b =>
        {
            b.Property(d => d.Dish).HasMaxLength(200);
            b.Property(d => d.Protein).HasColumnType("decimal(18,2)");
            b.Property(d => d.Carbohydrates).HasColumnType("decimal(18,2)");
            b.Property(d => d.Fat).HasColumnType("decimal(18,2)");
            b.Property(d => d.Sugars).HasColumnType("decimal(18,2)");
            b.HasOne(d => d.Menu)
                .WithMany(m => m.Days)
                .HasForeignKey(d => d.MenuId)
                .OnDelete(DeleteBehavior.Cascade);
            // Optional source recipe; if the recipe is deleted, keep the (copied) day values.
            b.HasOne(d => d.Recipe)
                .WithMany()
                .HasForeignKey(d => d.RecipeId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Recipe>(b =>
        {
            b.Property(r => r.Name).IsRequired().HasMaxLength(120);
            b.Property(r => r.Instructions).HasMaxLength(8000);
            b.Property(r => r.Protein).HasColumnType("decimal(18,2)");
            b.Property(r => r.Carbohydrates).HasColumnType("decimal(18,2)");
            b.Property(r => r.Fat).HasColumnType("decimal(18,2)");
            b.Property(r => r.Sugars).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<RecipeIngredient>(b =>
        {
            b.Property(ri => ri.Quantity).HasColumnType("decimal(18,3)");
            b.HasOne(ri => ri.Recipe)
                .WithMany(r => r.Ingredients)
                .HasForeignKey(ri => ri.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(ri => ri.Ingredient)
                .WithMany()
                .HasForeignKey(ri => ri.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Ingredient>(b =>
        {
            b.Property(i => i.Name).IsRequired().HasMaxLength(120);
            b.Property(i => i.Unit).IsRequired().HasMaxLength(20);
            b.Property(i => i.Quantity).HasColumnType("decimal(18,3)");
            b.Property(i => i.LowStockThreshold).HasColumnType("decimal(18,3)");
            // Normalized many-to-many with allergens.
            b.HasMany(i => i.Allergens)
                .WithMany(a => a.Ingredients)
                .UsingEntity(j => j.ToTable("IngredientAllergens"));
        });

        modelBuilder.Entity<Allergen>(b =>
        {
            b.Property(a => a.Name).IsRequired().HasMaxLength(60);
            b.HasIndex(a => a.Name).IsUnique();
        });

        modelBuilder.Entity<Employee>(b =>
        {
            b.Ignore(e => e.FullName);
            b.Property(e => e.FirstName).IsRequired().HasMaxLength(80);
            b.Property(e => e.LastName).IsRequired().HasMaxLength(80);
            b.Property(e => e.Role).IsRequired().HasMaxLength(40);
            b.Property(e => e.HourlyRate).HasColumnType("decimal(18,2)");
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
