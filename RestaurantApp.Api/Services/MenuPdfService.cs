using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Services;

namespace RestaurantApp.Api.Services;

/// <summary>Renders a weekly menu as an A4-landscape PDF (a Monday–Sunday grid of recipes).</summary>
public class MenuPdfService
{
    private static readonly string[] DayNames =
        ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];

    public byte[] Generate(Menu menu)
    {
        var recipesByDay = menu.MenuRecipes
            .GroupBy(mr => mr.Day)
            .ToDictionary(g => g.Key, g => g.Select(mr => mr.Recipe).ToList());

        var menuAllergens = AllergenSummary.ForMenu(menu);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Darken4));

                page.Header().Column(col =>
                {
                    col.Item().Text(menu.Name).FontSize(22).Bold();
                    col.Item().Text(
                        $"Week of {menu.WeekStart:dd MMM yyyy} – {menu.WeekStart.AddDays(6):dd MMM yyyy}")
                        .FontSize(12).FontColor(Colors.Grey.Darken1);
                });

                page.Content().PaddingVertical(12).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        for (var i = 0; i < 7; i++)
                            columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        for (var day = 0; day < 7; day++)
                        {
                            var date = menu.WeekStart.AddDays(day);
                            header.Cell().Element(HeaderCell).Column(c =>
                            {
                                c.Item().Text(DayNames[day]).Bold().FontColor(Colors.White);
                                c.Item().Text(date.ToString("dd.MM")).FontSize(8).FontColor(Colors.Grey.Lighten3);
                            });
                        }
                    });

                    for (var day = 0; day < 7; day++)
                    {
                        var recipes = recipesByDay.GetValueOrDefault(day) ?? [];
                        table.Cell().Element(BodyCell).Column(c =>
                        {
                            if (recipes.Count == 0)
                            {
                                c.Item().Text("—").FontColor(Colors.Grey.Medium);
                                return;
                            }

                            foreach (var recipe in recipes)
                            {
                                c.Item().PaddingBottom(6).Column(rc =>
                                {
                                    rc.Item().Text(recipe.Name).SemiBold();
                                    var allergens = AllergenSummary.ForRecipe(recipe);
                                    if (allergens.Count > 0)
                                    {
                                        rc.Item().Text($"Allergens: {string.Join(", ", allergens)}")
                                            .FontSize(7.5f).Italic().FontColor(Colors.Red.Darken1);
                                    }
                                });
                            }
                        });
                    }
                });

                page.Footer().PaddingTop(8).Column(col =>
                {
                    if (menuAllergens.Count > 0)
                        col.Item().Text($"Contains allergens: {string.Join(", ", menuAllergens)}").FontSize(9).Bold();
                    if (!string.IsNullOrWhiteSpace(menu.NutritionalInfo))
                        col.Item().Text(menu.NutritionalInfo).FontSize(8).FontColor(Colors.Grey.Darken1);
                    if (!string.IsNullOrWhiteSpace(menu.Content))
                        col.Item().Text(menu.Content).FontSize(8).FontColor(Colors.Grey.Darken1);
                    col.Item().PaddingTop(4).Text($"Generated {DateTime.Now:dd MMM yyyy HH:mm}")
                        .FontSize(7).FontColor(Colors.Grey.Medium);
                });
            });
        }).GeneratePdf();
    }

    private static IContainer HeaderCell(IContainer container) =>
        container.Background(Colors.Blue.Darken2).Padding(6);

    private static IContainer BodyCell(IContainer container) =>
        container.Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(6).MinHeight(120);
}
