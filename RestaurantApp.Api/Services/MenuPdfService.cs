using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RestaurantApp.Core.Entities;
using RestaurantApp.Core.Services;

namespace RestaurantApp.Api.Services;

/// <summary>Renders a weekly menu as an A4-landscape PDF (a Monday to Sunday grid of dishes + nutrition).</summary>
public class MenuPdfService
{
    private const string HeaderGreen = "#24513F";

    private static readonly string[] DayNamesEn =
        ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
    private static readonly string[] DayNamesFr =
        ["Lundi", "Mardi", "Mercredi", "Jeudi", "Vendredi", "Samedi", "Dimanche"];

    public byte[] Generate(Menu menu, string culture = "fr")
    {
        var fr = culture != "en";
        var dayNames = fr ? DayNamesFr : DayNamesEn;
        var weekOfLabel = fr ? "Semaine du" : "Week of";
        var allergensLabel = fr ? "Allergènes" : "Allergens";
        var containsLabel = fr ? "Contient les allergènes" : "Contains allergens";
        var generatedLabel = fr ? "Généré le" : "Generated";
        var proteinL = fr ? "Protéines" : "Protein";
        var carbsL = fr ? "Glucides" : "Carbs";
        var fatL = fr ? "Lipides" : "Fat";
        var sugarsL = fr ? "Sucres" : "Sugars";

        var daysByIndex = menu.Days
            .GroupBy(d => d.Day)
            .ToDictionary(g => g.Key, g => g.ToList());

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
                    col.Item().Text(menu.Name).FontSize(22).Bold().FontColor(HeaderGreen);
                    col.Item().Text(
                        $"{weekOfLabel} {menu.WeekStart:dd MMM yyyy} to {menu.WeekStart.AddDays(6):dd MMM yyyy}")
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
                                c.Item().Text(dayNames[day]).Bold().FontColor(Colors.White);
                                c.Item().Text(date.ToString("dd.MM")).FontSize(8).FontColor(Colors.Grey.Lighten3);
                            });
                        }
                    });

                    for (var day = 0; day < 7; day++)
                    {
                        var entries = daysByIndex.GetValueOrDefault(day) ?? [];
                        table.Cell().Element(BodyCell).Column(c =>
                        {
                            if (entries.Count == 0)
                            {
                                c.Item().Text("-").FontColor(Colors.Grey.Medium);
                                return;
                            }

                            foreach (var entry in entries)
                            {
                                c.Item().PaddingBottom(8).Column(rc =>
                                {
                                    rc.Item().Text(string.IsNullOrWhiteSpace(entry.Dish) ? "-" : entry.Dish).SemiBold();
                                    rc.Item().Text($"{entry.Calories} kcal").FontSize(8).FontColor(HeaderGreen);
                                    rc.Item().Text(
                                        $"{proteinL} {G(entry.Protein)} · {carbsL} {G(entry.Carbohydrates)} · " +
                                        $"{fatL} {G(entry.Fat)} · {sugarsL} {G(entry.Sugars)}")
                                        .FontSize(7.5f).FontColor(Colors.Grey.Darken1);

                                    var allergens = entry.Recipe is null ? [] : AllergenSummary.ForRecipe(entry.Recipe);
                                    if (allergens.Count > 0)
                                    {
                                        rc.Item().Text($"{allergensLabel}: {string.Join(", ", allergens)}")
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
                        col.Item().Text($"{containsLabel}: {string.Join(", ", menuAllergens)}").FontSize(9).Bold();
                    if (!string.IsNullOrWhiteSpace(menu.Content))
                        col.Item().Text(menu.Content).FontSize(8).FontColor(Colors.Grey.Darken1);
                    col.Item().PaddingTop(4).Text($"{generatedLabel} {DateTime.Now:dd MMM yyyy HH:mm}")
                        .FontSize(7).FontColor(Colors.Grey.Medium);
                });
            });
        }).GeneratePdf();
    }

    private static string G(decimal grams) => $"{grams.ToString("0.#")} g";

    private static IContainer HeaderCell(IContainer container) =>
        container.Background(HeaderGreen).Padding(6);

    private static IContainer BodyCell(IContainer container) =>
        container.Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(6).MinHeight(120);
}
