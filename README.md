# RestaurantApp

A small corporate-restaurant management demo built to showcase full-stack **.NET 8**
skills: an **ASP.NET Core Web API** backend with **EF Core / SQLite**, and a
**Blazor WebAssembly** frontend.

It models three everyday operations of a staff restaurant:

- **Menus** — daily menus made up of dishes, each dish consuming ingredients in set quantities.
- **Stock** — ingredient inventory with low-stock thresholds. Serving a menu deducts stock atomically.
- **Planning** — a weekly staff shift schedule.

## Tech stack

| Layer        | Technology                                   |
|--------------|----------------------------------------------|
| Backend API  | ASP.NET Core 8 Web API (controllers, `IResult`) |
| ORM          | Entity Framework Core 8 + SQLite             |
| Frontend     | Blazor WebAssembly (standalone)              |
| Tests        | xUnit + FluentAssertions                     |
| Language     | C# 12                                        |

## Solution structure

```
RestaurantApp.Core            Domain entities, repository interfaces, StockDeductionService
RestaurantApp.Infrastructure  EF Core DbContext, value converters, repositories, seeding, migrations
RestaurantApp.Api             Web API controllers + DTOs (Menus, Ingredients, Shifts, Employees)
RestaurantApp.Web             Blazor WebAssembly UI (Menus, Stock, Planning)
RestaurantApp.Tests           Unit tests for the stock-deduction business logic
```

The architecture is a clean, layered **Repository + Unit of Work** design — deliberately
simpler than CQRS/mediator, to stay readable for a demo. `Core` has no infrastructure
dependencies; the repository and transaction abstractions live there and are implemented in
`Infrastructure`.

## Key business logic — `StockDeductionService`

When a menu is served (`POST /api/menus/{id}/serve`), the service:

1. Loads the menu's items and their ingredients.
2. Aggregates the required quantity per ingredient (`QuantityRequired * servings`).
3. **Validates the whole order first** — if any ingredient is short, the transaction is rolled
   back and **no stock is deducted**.
4. Deducts stock and returns a **low-stock warning** for any ingredient that drops below its threshold.

All deductions run inside a single EF Core transaction. This is the primary unit-test target.

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

The tests use an **in-memory SQLite** database (not the EF in-memory provider) so that real
transactions — and therefore the rollback behaviour — are exercised.

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
| GET    | `/api/menus` | List all menus |
| GET    | `/api/menus/today` | Today's active menus |
| GET    | `/api/menus/{id}` | Get a menu |
| POST   | `/api/menus` | Create a menu |
| PUT    | `/api/menus/{id}` | Update a menu |
| DELETE | `/api/menus/{id}` | Delete a menu |
| POST   | `/api/menus/{id}/serve` | Serve a menu (deduct stock, transactional) |
| GET    | `/api/ingredients` | List ingredients with stock |
| GET    | `/api/ingredients/low-stock` | Ingredients below threshold |
| GET    | `/api/ingredients/{id}` | Get an ingredient |
| POST   | `/api/ingredients` | Create an ingredient |
| PATCH  | `/api/ingredients/{id}/stock` | Update stock quantity |
| GET    | `/api/shifts` | List all shifts |
| GET    | `/api/shifts/week?start={date}` | Shifts for a week |
| GET    | `/api/shifts/employee/{id}` | Shifts for one employee |
| POST   | `/api/shifts` | Create a shift |
| PUT    | `/api/shifts/{id}` | Update a shift |
| DELETE | `/api/shifts/{id}` | Delete a shift |
| GET    | `/api/employees` | List employees |
