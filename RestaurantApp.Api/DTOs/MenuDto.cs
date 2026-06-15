namespace RestaurantApp.Api.DTOs;

public record MenuDto(int Id, string Name, DateOnly WeekStart, string Content, string NutritionalInfo);

/// <summary>Lightweight row for the menu list (omits the long text fields).</summary>
public record MenuSummaryDto(int Id, string Name, DateOnly WeekStart);

public record CreateMenuRequest(string Name, DateOnly WeekStart);

public record UpdateMenuRequest(string Name, DateOnly WeekStart, string Content, string NutritionalInfo);
