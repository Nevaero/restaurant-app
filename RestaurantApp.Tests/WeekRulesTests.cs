using FluentAssertions;
using RestaurantApp.Core;
using Xunit;

namespace RestaurantApp.Tests;

public class WeekRulesTests
{
    [Theory]
    [InlineData("2026-06-15", true)]   // Monday
    [InlineData("2026-06-16", false)]  // Tuesday
    [InlineData("2026-06-21", false)]  // Sunday
    public void IsMonday_DetectsMondays(string date, bool expected)
    {
        WeekRules.IsMonday(DateOnly.Parse(date)).Should().Be(expected);
    }

    [Theory]
    [InlineData("2026-06-15", "2026-06-15")] // Monday → itself
    [InlineData("2026-06-17", "2026-06-15")] // Wednesday → that Monday
    [InlineData("2026-06-21", "2026-06-15")] // Sunday → that Monday
    public void MondayOf_SnapsToStartOfWeek(string input, string expected)
    {
        WeekRules.MondayOf(DateOnly.Parse(input)).Should().Be(DateOnly.Parse(expected));
    }
}
