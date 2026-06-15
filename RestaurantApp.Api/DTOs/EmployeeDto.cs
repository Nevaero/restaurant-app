namespace RestaurantApp.Api.DTOs;

public record EmployeeDto(int Id, string FirstName, string LastName, string Role);

public record CreateEmployeeRequest(string FirstName, string LastName, string Role);
