namespace RestaurantApp.Api.DTOs;

public record ShiftDto(
    int Id,
    int EmployeeId,
    string EmployeeName,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string? Notes);

public record CreateShiftRequest(
    int EmployeeId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string? Notes);

public record UpdateShiftRequest(
    int EmployeeId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string? Notes);
