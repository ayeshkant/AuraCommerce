using AuraCommerce.Orders.Application.Interfaces;
using AuraCommerce.Orders.Application.Services;
using AuraCommerce.Orders.Domain.Interfaces;
using AuraCommerce.Orders.Infrastructure.Client;
using AuraCommerce.Orders.Infrastructure.Context;
using AuraCommerce.Orders.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("OrderDb");

// Add services to the container.
builder.Services.AddDbContext<OrderDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddHttpClient<ICatalogServiceClient, CatalogHttpClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["CatalogApi:BaseUrl"]!);
}).AddStandardResilienceHandler();
builder.Services.AddScoped<OrderService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
