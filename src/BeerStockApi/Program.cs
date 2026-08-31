using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BeerStockApi.Infrastructure.BeerStockApiDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("BeerStockDatabase")));

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
    .WithName("GetHealth");

app.Run();
