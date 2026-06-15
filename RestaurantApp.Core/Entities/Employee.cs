namespace RestaurantApp.Core.Entities;

/// <summary>A restaurant staff member.</summary>
public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;     // "Cook", "Clerk", "Server", "Manager"
    public decimal HourlyRate { get; set; }              // used for labour-cost reporting
    public ICollection<Shift> Shifts { get; set; } = [];

    public string FullName => $"{FirstName} {LastName}";
}
