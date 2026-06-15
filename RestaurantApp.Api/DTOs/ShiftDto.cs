namespace RestaurantApp.Api.DTOs;

public record ShiftDto(
    int Id,
    int EmployeeId,
    string EmployeeName,
    string Role,
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

public record EmployeeWeekSummaryDto(
    int EmployeeId,
    string Name,
    string Role,
    decimal TotalHours,
    decimal LaborCost,
    bool IsOvertime);

/// <summary>The full week view: the Monday it starts on, all shifts, and per-employee totals.</summary>
public record WeekScheduleDto(
    DateOnly WeekStart,
    IReadOnlyList<ShiftDto> Shifts,
    IReadOnlyList<EmployeeWeekSummaryDto> Summaries);
