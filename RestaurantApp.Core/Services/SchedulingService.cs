using RestaurantApp.Core.Entities;

namespace RestaurantApp.Core.Services;

/// <summary>Per-employee totals for a single planning week.</summary>
public record EmployeeWeekSummary(
    int EmployeeId,
    string Name,
    string Role,
    decimal TotalHours,
    decimal LaborCost,
    bool IsOvertime);

/// <summary>
/// Pure scheduling rules for the weekly planning grid: shift validation, weekly hours,
/// labour cost and overtime flagging. Kept dependency-free so it is trivially unit-tested.
/// </summary>
public class SchedulingService
{
    /// <summary>Weekly hours beyond which a staff member is flagged as in overtime.</summary>
    public const decimal OvertimeThresholdHours = 42m;

    /// <summary>Duration of a single shift in hours.</summary>
    public static decimal Hours(Shift shift) =>
        (decimal)(shift.EndTime.ToTimeSpan() - shift.StartTime.ToTimeSpan()).TotalHours;

    /// <summary>True when two shifts are for the same employee, same day, and their times overlap.</summary>
    public static bool Overlaps(Shift a, Shift b) =>
        a.EmployeeId == b.EmployeeId
        && a.Date == b.Date
        && a.StartTime < b.EndTime
        && b.StartTime < a.EndTime;

    /// <summary>
    /// Validates a shift against the employee's existing shifts. Returns an error message,
    /// or null when the shift is valid.
    /// </summary>
    public string? ValidateShift(Shift candidate, IEnumerable<Shift> existingForEmployee)
    {
        if (candidate.EndTime <= candidate.StartTime)
            return "End time must be after start time.";

        if (existingForEmployee.Any(existing => existing.Id != candidate.Id && Overlaps(existing, candidate)))
            return "This shift overlaps an existing shift for the same employee.";

        return null;
    }

    /// <summary>Summarises weekly hours, labour cost and overtime for each employee.</summary>
    public IReadOnlyList<EmployeeWeekSummary> SummariseWeek(
        IEnumerable<Employee> employees,
        IEnumerable<Shift> weekShifts)
    {
        var hoursByEmployee = weekShifts
            .GroupBy(s => s.EmployeeId)
            .ToDictionary(g => g.Key, g => g.Sum(Hours));

        return employees
            .Select(e =>
            {
                var hours = hoursByEmployee.GetValueOrDefault(e.Id, 0m);
                return new EmployeeWeekSummary(
                    e.Id,
                    e.FullName,
                    e.Role,
                    hours,
                    Math.Round(hours * e.HourlyRate, 2),
                    hours > OvertimeThresholdHours);
            })
            .ToList();
    }
}
