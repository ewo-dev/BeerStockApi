using Microsoft.EntityFrameworkCore;
using BeerStockApi.Infrastructure;
using BeerStockApi.Endpoints;
using BeerStockApi.Repositories;
using BeerStockApi.Services;
using BeerStockApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BeerStockApi.Infrastructure.BeerStockApiDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("BeerStockDatabase")));

builder.Services.AddScoped<IBeerRepository, BeerRepository>();
builder.Services.AddScoped<IWholesalerBeerRepository, WholesalerBeerRepository>();
builder.Services.AddScoped<IWholesalerRepository, WholesalerRepository>();

builder.Services.AddScoped<IBeerService, BeerService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IQuoteService, QuoteService>();

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

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.MapBeerEndpoints();
app.MapStockEndpoints();
app.MapQuoteEndpoints();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
    .WithName("GetHealth");

app.Run();
