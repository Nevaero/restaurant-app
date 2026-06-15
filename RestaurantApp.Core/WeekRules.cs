namespace RestaurantApp.Core;

/// <summary>Helpers for the Monday-anchored week used by menus and the planning grid.</summary>
public static class WeekRules
{
    public static bool IsMonday(DateOnly date) => date.DayOfWeek == DayOfWeek.Monday;

    /// <summary>Returns the Monday of the week containing <paramref name="date"/>.</summary>
    public static DateOnly MondayOf(DateOnly date) =>
        date.AddDays(-(((int)date.DayOfWeek + 6) % 7));
}
