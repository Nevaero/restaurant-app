# RestaurantApp

A lightweight restaurant ERP (MVP) built to showcase full-stack **.NET 8** skills:
an **ASP.NET Core Web API** backend with **EF Core / SQLite**, and a **Blazor
WebAssembly** frontend.

The homepage is a card grid leading to the modules:

- **Menu** — weekly menus (Monday–Sunday). You pick the Monday with a date picker, give it a
  name, **load recipes onto days**, and **export the menu as an A4-landscape PDF** (a
  Monday–Sunday grid with per-day recipes and aggregated allergens).
- **Recipes** — a searchable recipe list with full CRUD. Recipes are built from inventory
  ingredients (with quantities); their allergens roll up automatically from those ingredients.
- **Planning** — the weekly staff schedule for cooks, clerks, servers and managers, with
  per-employee hours, labour cost and overtime alerts (informed by how tools like
  7shifts/Deputy structure schedules).
- **Inventaire** (inventory) — ingredients with quantities, units, low-stock alerts and
  **normalized allergens** (the EU's 14 major allergens, modelled as their own table and a
  many-to-many relationship).

Menu PDFs are generated server-side with **QuestPDF**.

The UI is **bilingual (French / English)** with **French as the default**. A language switcher
in the top bar flips the whole UI live (no reload) and remembers the choice in `localStorage`;
the menu PDF is generated in the selected language too.

## Tech stack

| Layer        | Technology                                       |
|--------------|--------------------------------------------------|
| Backend API  | ASP.NET Core 8 Web API (controllers, `IResult`)  |
| ORM          | Entity Framework Core 8 + SQLite                 |
| Frontend     | Blazor WebAssembly (standalone)                  |
| PDF export   | QuestPDF (Community licence)                     |
| Tests        | xUnit + FluentAssertions                         |
| Language     | C# 12                                            |

## Solution structure

```
RestaurantApp.Core            Domain entities, repository interfaces, SchedulingService,
                              AllergenSummary, WeekRules
RestaurantApp.Infrastructure  EF Core DbContext, value converters, repositories, seeding, migrations
RestaurantApp.Api             Web API controllers + record DTOs; QuestPDF menu export
RestaurantApp.Web             Blazor WebAssembly UI (Menu, Recipes, Planning, Inventaire)
RestaurantApp.Tests           Unit tests for scheduling, allergen aggregation and week handling
```

The architecture is a clean, layered **Repository + Unit of Work** design — deliberately
simpler than CQRS/mediator, to stay readable for a demo. `Core` has no infrastructure
dependencies.

## Key business logic

- **`SchedulingService`** (Core) — pure scheduling rules: shift duration, overlap detection,
  shift validation (no overlapping shifts for the same employee), and a weekly summary per
  employee (total hours, labour cost = hours × hourly rate, and an overtime flag above
  42 h/week). This is the primary unit-test target.
- **`AllergenSummary`** (Core) — rolls allergens up the graph: from an ingredient's normalized
  allergens to a recipe (distinct union of its ingredients), and from a recipe to a whole menu.
- **`WeekRules`** (Core) — Monday-anchored week helpers. Menus and the planning grid both run
  Monday–Sunday; any date the user picks for a menu is snapped to that week's Monday.

## Running locally

Prerequisites: the **.NET 8 SDK**.

```bash
# Restore + build everything
dotnet build

# Run the API (http://localhost:5000, Swagger UI at /swagger)
# Migrations are applied and demo data is seeded automatically on startup.
cd RestaurantApp.Api
dotnet run

# In a second terminal, run the Blazor frontend (http://localhost:5003)
cd RestaurantApp.Web
dotnet run
```

The frontend reads the API base URL from `RestaurantApp.Web/wwwroot/appsettings.json`
(`ApiBaseUrl`, default `http://localhost:5000`).

> HTTPS profiles are also configured (API `https://localhost:5001`, Web `https://localhost:5002`).
> To use them, run `dotnet dev-certs https --trust` first and point `ApiBaseUrl` at the HTTPS API URL.

## Deploying with Docker

The app ships as two containers, wired together by `docker-compose.yml`:

- **api** — the ASP.NET Core API (Ubuntu-based .NET 8 runtime). SQLite lives on a named
  volume (`restaurant-data`) so data survives restarts; migrations run and demo data is
  seeded automatically on first start.
- **web** — nginx serving the published Blazor WebAssembly app and reverse-proxying `/api`
  to the api container. Because the browser talks to a single origin, there is no CORS to
  configure and no API hostname to hard-code.

On the VPS:

```bash
docker compose up -d --build
```

Then browse to `http://<vps-ip>:8080`. Only the web container publishes a port; the api is
reachable only on the internal compose network. For production, terminate TLS with a reverse
proxy (Caddy, Traefik, or nginx) in front of the web container, or map it to port 80/443.

To build the images individually (build context is the repo root):

```bash
docker build -f RestaurantApp.Api/Dockerfile -t restaurantapp-api .
docker build -f RestaurantApp.Web/Dockerfile -t restaurantapp-web .
```

## Running the tests

```bash
dotnet test
```

## Database & migrations

- SQLite for development (`restaurant.db`, created in the API project folder on first run).
- `DateOnly` / `TimeOnly` are stored as ISO text via custom EF Core value converters.
- To add a migration:

  ```bash
  dotnet ef migrations add <Name> \
    --project RestaurantApp.Infrastructure \
    --startup-project RestaurantApp.Api \
    --output-dir Data/Migrations
  ```

## API overview

| Method | Route | Description |
|--------|-------|-------------|
| GET    | `/api/menus` | List menus (summary) |
| GET    | `/api/menus/{id}` | Get a menu with content |
| POST   | `/api/menus` | Create a menu (week snapped to Monday) |
| PUT    | `/api/menus/{id}` | Update a menu (name, week, content, nutrition, recipe assignments) |
| DELETE | `/api/menus/{id}` | Delete a menu |
| GET    | `/api/menus/{id}/pdf` | Export the menu as an A4-landscape PDF |
| GET    | `/api/recipes?search={term}` | Search recipes (by name, instructions or ingredient) |
| GET    | `/api/recipes/{id}` | Get a recipe with ingredients + allergens |
| POST   | `/api/recipes` | Create a recipe |
| PUT    | `/api/recipes/{id}` | Update a recipe |
| DELETE | `/api/recipes/{id}` | Delete a recipe |
| GET    | `/api/allergens` | List the normalized allergens |
| GET    | `/api/ingredients` | List inventory items |
| GET    | `/api/ingredients/low-stock` | Items below their threshold |
| GET    | `/api/ingredients/{id}` | Get an item |
| POST   | `/api/ingredients` | Create an item |
| PUT    | `/api/ingredients/{id}` | Update an item |
| DELETE | `/api/ingredients/{id}` | Delete an item |
| GET    | `/api/shifts/week?start={date}` | Shifts for a week |
| GET    | `/api/shifts/week/schedule?start={date}` | Week view: shifts + per-employee summary |
| POST   | `/api/shifts` | Create a shift (overlap-validated) |
| PUT    | `/api/shifts/{id}` | Update a shift |
| DELETE | `/api/shifts/{id}` | Delete a shift |
| GET    | `/api/employees` | List employees |
| POST   | `/api/employees` | Create an employee |
