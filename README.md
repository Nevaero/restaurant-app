# RestaurantApp

A lightweight restaurant ERP (MVP) built to showcase full-stack **.NET 8** skills:
an **ASP.NET Core Web API** backend with **EF Core / SQLite**, and a **Blazor
WebAssembly** frontend.

The homepage is a three-card grid leading to the three modules:

- **Menu** — weekly menus. Each menu runs Monday–Sunday; you pick the Monday with a
  date picker and give it a name, then edit its free-text content and nutritional values.
- **Planning** — the weekly staff schedule for cooks, clerks, servers and managers, with
  per-employee hours, labour cost and overtime alerts (informed by how tools like
  7shifts/Deputy structure schedules).
- **Inventaire** (inventory) — ingredients with quantities, units, **allergens** (the EU's
  14 major allergens) and low-stock alerts.

## Tech stack

| Layer        | Technology                                       |
|--------------|--------------------------------------------------|
| Backend API  | ASP.NET Core 8 Web API (controllers, `IResult`)  |
| ORM          | Entity Framework Core 8 + SQLite                 |
| Frontend     | Blazor WebAssembly (standalone)                  |
| Tests        | xUnit + FluentAssertions                         |
| Language     | C# 12                                            |

## Solution structure

```
RestaurantApp.Core            Domain entities, repository interfaces, SchedulingService, WeekRules
RestaurantApp.Infrastructure  EF Core DbContext, value converters, repositories, seeding, migrations
RestaurantApp.Api             Web API controllers + record DTOs (Menus, Ingredients, Shifts, Employees)
RestaurantApp.Web             Blazor WebAssembly UI (Menu, Planning, Inventaire)
RestaurantApp.Tests           Unit tests for the scheduling rules and week handling
```

The architecture is a clean, layered **Repository + Unit of Work** design — deliberately
simpler than CQRS/mediator, to stay readable for a demo. `Core` has no infrastructure
dependencies.

## Key business logic

- **`SchedulingService`** (Core) — pure scheduling rules: shift duration, overlap detection,
  shift validation (no overlapping shifts for the same employee), and a weekly summary per
  employee (total hours, labour cost = hours × hourly rate, and an overtime flag above
  42 h/week). This is the primary unit-test target.
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
| PUT    | `/api/menus/{id}` | Update a menu's name, week, content, nutrition |
| DELETE | `/api/menus/{id}` | Delete a menu |
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
