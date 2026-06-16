namespace RestaurantApp.Web.Services;

/// <summary>
/// Lightweight client-side i18n. Holds French/English strings and the active culture,
/// and notifies the layout to re-render when the language changes. French is the default.
/// </summary>
public class LocalizationService
{
    public const string French = "fr";
    public const string English = "en";

    private string _culture = French;

    /// <summary>Raised when the active culture changes so the UI can re-render.</summary>
    public event Action? OnChange;

    public string Culture
    {
        get => _culture;
        private set
        {
            if (_culture == value) return;
            _culture = value;
            OnChange?.Invoke();
        }
    }

    public bool IsFrench => _culture == French;

    public void SetCulture(string culture) =>
        Culture = culture == English ? English : French;

    /// <summary>Localized lookup. Falls back to the key if no translation exists.</summary>
    public string this[string key] =>
        Translations.TryGetValue(key, out var pair)
            ? (IsFrench ? pair.Fr : pair.En)
            : key;

    /// <summary>Localized day name (0 = Monday … 6 = Sunday).</summary>
    public string Day(int index) => this[$"day.{index}"];

    /// <summary>Localized role label, falling back to the stored value.</summary>
    public string Role(string role) => this[$"role.{role}"] is var r && r != $"role.{role}" ? r : role;

    private static readonly Dictionary<string, (string Fr, string En)> Translations = new()
    {
        // App shell / navigation
        ["app.title"] = ("EasyKitchen", "EasyKitchen"),
        ["app.tagline"] = ("Un ERP léger pour cuisines professionnelles : menus, recettes, planning et inventaire.",
                           "A lightweight ERP for professional kitchens: menus, recipes, planning and inventory."),
        ["nav.home"] = ("Accueil", "Home"),
        ["nav.menu"] = ("Menu", "Menu"),
        ["nav.recipes"] = ("Recettes", "Recipes"),
        ["nav.planning"] = ("Planning", "Planning"),
        ["nav.inventory"] = ("Inventaire", "Inventory"),
        ["nav.language"] = ("Langue", "Language"),

        // Common
        ["common.add"] = ("Ajouter", "Add"),
        ["common.edit"] = ("Modifier", "Edit"),
        ["common.delete"] = ("Supprimer", "Delete"),
        ["common.cancel"] = ("Annuler", "Cancel"),
        ["common.save"] = ("Enregistrer", "Save"),
        ["common.done"] = ("Terminé", "Done"),
        ["common.back"] = ("Retour", "Back"),
        ["common.open"] = ("Ouvrir", "Open"),
        ["common.name"] = ("Nom", "Name"),
        ["common.loading"] = ("Chargement…", "Loading…"),
        ["common.status"] = ("Statut", "Status"),
        ["common.nameRequired"] = ("Le nom est obligatoire.", "Name is required."),

        // Home cards
        ["home.menu.desc"] = ("Planifiez les menus hebdomadaires, placez des recettes par jour et exportez un PDF A4.",
                              "Plan weekly menus, load recipes onto days, and export an A4 PDF."),
        ["home.recipes.desc"] = ("Composez des recettes à partir des ingrédients ; les allergènes se cumulent automatiquement.",
                                 "Build recipes from inventory ingredients; allergens roll up automatically."),
        ["home.planning.desc"] = ("Planifiez cuisiniers et employés, avec heures, coût salarial et alertes d'heures supplémentaires.",
                                  "Schedule cooks and clerks, with hours, labour cost and overtime alerts."),
        ["home.inventory.desc"] = ("Suivez les ingrédients, quantités et allergènes, avec alertes de stock bas.",
                                   "Track ingredients, quantities and allergens, with low-stock alerts."),

        // Menus
        ["menus.title"] = ("Menus hebdomadaires", "Weekly Menus"),
        ["menus.new"] = ("+ Nouveau menu", "+ New menu"),
        ["menus.empty"] = ("Aucun menu pour l'instant. Créez-en un pour commencer.", "No menus yet. Create one to get started."),
        ["menus.weekOf"] = ("Semaine du (lundi)", "Week of (Monday)"),
        ["menus.newMenu"] = ("Nouveau menu", "New menu"),
        ["menus.weekStarting"] = ("Semaine commençant le (lundi)", "Week starting (Monday)"),
        ["menus.snapHint"] = ("Toute date choisie est ramenée au lundi de cette semaine.", "Any date you pick is snapped to that week's Monday."),
        ["menus.createEdit"] = ("Créer et modifier", "Create & edit"),

        // Menu editor
        ["menuEdit.title"] = ("Modifier le menu", "Edit menu"),
        ["menuEdit.notFound"] = ("Menu introuvable.", "Menu not found."),
        ["menuEdit.downloadPdf"] = ("⤓ Télécharger le PDF (A4)", "⤓ Download PDF (A4)"),
        ["menuEdit.dayHint"] = ("Un onglet par jour. Importez une recette pour copier ses valeurs dans le formulaire du jour, puis ajustez si besoin.",
                                "One tab per day. Import a recipe to copy its values into the day's form, then adjust if needed."),
        ["menuEdit.dish"] = ("Plat", "Dish"),
        ["menuEdit.importRecipe"] = ("Importer une recette", "Import a recipe"),
        ["menuEdit.import"] = ("Importer", "Import"),
        ["menuEdit.clearDay"] = ("Vider ce jour", "Clear this day"),
        ["menuEdit.allergensAcross"] = ("Allergènes de ce menu :", "Allergens across this menu:"),
        ["menuEdit.dayAllergens"] = ("Allergènes du jour :", "Day allergens:"),
        ["menuEdit.notes"] = ("Notes", "Notes"),
        ["menuEdit.saved"] = ("Enregistré.", "Saved."),
        ["common.select"] = ("Choisir…", "Select…"),

        // Nutrition
        ["nutrition.title"] = ("Valeurs nutritionnelles", "Nutritional values"),
        ["nutrition.perServing"] = ("par portion", "per serving"),
        ["nutrition.calories"] = ("Calories", "Calories"),
        ["nutrition.protein"] = ("Protéines", "Protein"),
        ["nutrition.carbohydrates"] = ("Glucides", "Carbohydrates"),
        ["nutrition.fat"] = ("Lipides", "Fat"),
        ["nutrition.sugars"] = ("Sucres", "Sugars"),

        // Recipes
        ["recipes.title"] = ("Recettes", "Recipes"),
        ["recipes.search"] = ("Rechercher…", "Search…"),
        ["recipes.new"] = ("+ Nouvelle recette", "+ New recipe"),
        ["recipes.none"] = ("Aucune recette trouvée.", "No recipes found."),
        ["recipes.servings"] = ("Portions", "Servings"),
        ["recipes.ingredients"] = ("Ingrédients", "Ingredients"),
        ["recipes.allergens"] = ("Allergènes", "Allergens"),
        ["recipes.newRecipe"] = ("Nouvelle recette", "New recipe"),
        ["recipes.editRecipe"] = ("Modifier la recette", "Edit recipe"),
        ["recipes.instructions"] = ("Préparation", "Instructions"),
        ["recipes.ingredient"] = ("Ingrédient", "Ingredient"),
        ["recipes.quantity"] = ("Quantité", "Quantity"),
        ["recipes.unit"] = ("Unité", "Unit"),
        ["recipes.addIngredient"] = ("+ Ajouter un ingrédient", "+ Add ingredient"),

        // Inventory
        ["inventory.title"] = ("Inventaire", "Inventory"),
        ["inventory.summary"] = ("article(s)", "item(s)"),
        ["inventory.lowCount"] = ("en stock bas", "low on stock"),
        ["inventory.addItem"] = ("+ Ajouter un article", "+ Add item"),
        ["inventory.quantity"] = ("Quantité", "Quantity"),
        ["inventory.unit"] = ("Unité", "Unit"),
        ["inventory.threshold"] = ("Seuil de stock bas", "Low-stock threshold"),
        ["inventory.allergens"] = ("Allergènes", "Allergens"),
        ["inventory.addTitle"] = ("Ajouter un article", "Add item"),
        ["inventory.editTitle"] = ("Modifier l'article", "Edit item"),
        ["inventory.low"] = ("Bas", "Low"),
        ["inventory.ok"] = ("OK", "OK"),

        // Planning
        ["planning.title"] = ("Planning hebdomadaire", "Weekly Schedule"),
        ["planning.previous"] = ("← Précédent", "← Previous"),
        ["planning.next"] = ("Suivant →", "Next →"),
        ["planning.thisWeek"] = ("Cette semaine", "This week"),
        ["planning.addEmployee"] = ("+ Ajouter un employé", "+ Add employee"),
        ["planning.employee"] = ("Employé", "Employee"),
        ["planning.summary"] = ("Résumé de la semaine", "Week summary"),
        ["planning.role"] = ("Rôle", "Role"),
        ["planning.hours"] = ("Heures", "Hours"),
        ["planning.laborCost"] = ("Coût salarial", "Labour cost"),
        ["planning.total"] = ("Total", "Total"),
        ["planning.overtime"] = ("Heures supp.", "Overtime"),
        ["planning.overtimeNote"] = ("Les heures supplémentaires sont signalées au-delà de 42 h/semaine.",
                                     "Overtime is flagged above 42 h/week."),
        ["planning.addShift"] = ("Ajouter un créneau", "Add shift"),
        ["planning.start"] = ("Début", "Start"),
        ["planning.end"] = ("Fin", "End"),
        ["planning.notesOptional"] = ("Notes (facultatif)", "Notes (optional)"),
        ["planning.firstName"] = ("Prénom", "First name"),
        ["planning.lastName"] = ("Nom", "Last name"),
        ["planning.hourlyRate"] = ("Taux horaire", "Hourly rate"),

        // Roles
        ["role.Cook"] = ("Cuisinier", "Cook"),
        ["role.Clerk"] = ("Employé", "Clerk"),
        ["role.Server"] = ("Serveur", "Server"),
        ["role.Manager"] = ("Manager", "Manager"),

        // Days (0 = Monday)
        ["day.0"] = ("Lundi", "Monday"),
        ["day.1"] = ("Mardi", "Tuesday"),
        ["day.2"] = ("Mercredi", "Wednesday"),
        ["day.3"] = ("Jeudi", "Thursday"),
        ["day.4"] = ("Vendredi", "Friday"),
        ["day.5"] = ("Samedi", "Saturday"),
        ["day.6"] = ("Dimanche", "Sunday"),
    };
}
