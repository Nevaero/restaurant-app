namespace RestaurantApp.Core.Entities;

/// <summary>A restaurant staff member.</summary>
public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;      // "Chef", "Server", "Manager"
    public ICollection<Shift> Shifts { get; set; } = [];
}
