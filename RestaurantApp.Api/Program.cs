using Microsoft.EntityFrameworkCore;
using RestaurantApp.Infrastructure;
using RestaurantApp.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

const string BlazorCorsPolicy = "BlazorClient";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "RestaurantApp API", Version = "v1" });
});

// EF Core + repositories + domain services.
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? "Data Source=restaurant.db";
builder.Services.AddInfrastructure(connectionString);

// Allow the standalone Blazor WebAssembly client to call the API during development.
builder.Services.AddCors(options =>
{
    options.AddPolicy(BlazorCorsPolicy, policy => policy
        .WithOrigins(
            builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                ?? ["https://localhost:5002", "http://localhost:5003"])
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

// Apply migrations and seed demo data on startup.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbInitializer.SeedAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(BlazorCorsPolicy);
app.UseAuthorization();
app.MapControllers();

app.Run();

// Exposed so the integration/WebApplicationFactory test host can reference the entry point.
public partial class Program;
