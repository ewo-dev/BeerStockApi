using Microsoft.EntityFrameworkCore;
using BeerStockApi.Infrastructure;
using BeerStockApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BeerStockApi.Infrastructure.BeerStockApiDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("BeerStockDatabase")));

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BeerStockApiDbContext>();
    await DbInitializer.InitializeAsync(dbContext);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapBeerEndpoints();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
    .WithName("GetHealth");

app.Run();
