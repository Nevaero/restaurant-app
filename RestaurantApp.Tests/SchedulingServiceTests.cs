using FluentAssertions;
using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Services;
using Xunit;

namespace RestaurantApp.Tests;

public class SchedulingServiceTests
{
    private readonly SchedulingService _service = new();

    private static Shift Shift(int id, int employeeId, string date, string start, string end) => new()
    {
        Id = id,
        EmployeeId = employeeId,
        Date = DateOnly.Parse(date),
        StartTime = TimeOnly.Parse(start),
        EndTime = TimeOnly.Parse(end),
    };

    [Fact]
    public void Hours_ComputesShiftDuration()
    {
        SchedulingService.Hours(Shift(1, 1, "2026-06-15", "08:00", "16:30")).Should().Be(8.5m);
    }

    [Fact]
    public void Overlaps_IsTrue_ForSameEmployeeSameDayOverlappingTimes()
    {
        var a = Shift(1, 1, "2026-06-15", "08:00", "16:00");
        var b = Shift(2, 1, "2026-06-15", "14:00", "22:00");
        SchedulingService.Overlaps(a, b).Should().BeTrue();
    }

    [Fact]
    public void Overlaps_IsFalse_ForAdjacentShifts()
    {
        var a = Shift(1, 1, "2026-06-15", "08:00", "14:00");
        var b = Shift(2, 1, "2026-06-15", "14:00", "22:00");
        SchedulingService.Overlaps(a, b).Should().BeFalse();
    }

    [Fact]
    public void Overlaps_IsFalse_ForDifferentEmployees()
    {
        var a = Shift(1, 1, "2026-06-15", "08:00", "16:00");
        var b = Shift(2, 2, "2026-06-15", "08:00", "16:00");
        SchedulingService.Overlaps(a, b).Should().BeFalse();
    }

    [Fact]
    public void ValidateShift_RejectsEndBeforeStart()
    {
        var shift = Shift(0, 1, "2026-06-15", "16:00", "08:00");
        _service.ValidateShift(shift, []).Should().Contain("after start time");
    }

    [Fact]
    public void ValidateShift_RejectsOverlapWithExistingShift()
    {
        var existing = new[] { Shift(1, 1, "2026-06-15", "08:00", "16:00") };
        var candidate = Shift(0, 1, "2026-06-15", "12:00", "20:00");
        _service.ValidateShift(candidate, existing).Should().Contain("overlaps");
    }

    [Fact]
    public void ValidateShift_AllowsNonOverlappingShift()
    {
        var existing = new[] { Shift(1, 1, "2026-06-15", "08:00", "14:00") };
        var candidate = Shift(0, 1, "2026-06-15", "14:00", "20:00");
        _service.ValidateShift(candidate, existing).Should().BeNull();
    }

    [Fact]
    public void ValidateShift_IgnoresTheShiftBeingEdited()
    {
        // Editing the same shift (same Id) must not conflict with itself.
        var existing = new[] { Shift(5, 1, "2026-06-15", "08:00", "16:00") };
        var candidate = Shift(5, 1, "2026-06-15", "09:00", "17:00");
        _service.ValidateShift(candidate, existing).Should().BeNull();
    }

    [Fact]
    public void SummariseWeek_ComputesHoursCostAndOvertime()
    {
        var employees = new[]
        {
            new Employee { Id = 1, FirstName = "Alice", LastName = "M", Role = "Cook", HourlyRate = 30m },
            new Employee { Id = 2, FirstName = "Bruno", LastName = "R", Role = "Clerk", HourlyRate = 25m },
        };

        // Alice: 5 × 9h = 45h (overtime, >42). Bruno: one 8h shift.
        var shifts = new List<Shift>();
        for (var d = 0; d < 5; d++)
            shifts.Add(Shift(d + 1, 1, $"2026-06-{15 + d:00}", "08:00", "17:00"));
        shifts.Add(Shift(99, 2, "2026-06-15", "09:00", "17:00"));

        var summaries = _service.SummariseWeek(employees, shifts);

        var alice = summaries.Single(s => s.EmployeeId == 1);
        alice.TotalHours.Should().Be(45m);
        alice.LaborCost.Should().Be(1350m); // 45 × 30
        alice.IsOvertime.Should().BeTrue();

        var bruno = summaries.Single(s => s.EmployeeId == 2);
        bruno.TotalHours.Should().Be(8m);
        bruno.LaborCost.Should().Be(200m); // 8 × 25
        bruno.IsOvertime.Should().BeFalse();
    }

    [Fact]
    public void SummariseWeek_IncludesEmployeesWithNoShifts()
    {
        var employees = new[] { new Employee { Id = 1, FirstName = "Alice", LastName = "M", Role = "Cook", HourlyRate = 30m } };
        var summaries = _service.SummariseWeek(employees, []);
        summaries.Single().TotalHours.Should().Be(0m);
        summaries.Single().LaborCost.Should().Be(0m);
    }
}
