using AuraCommerce.IdentityApi.Infrastructure.Data;
using Azure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Initializing AuraCommerce Identity Service ...");
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithProcessId()
        .Enrich.WithProperty("MicroserviceContext", builder.Environment.ApplicationName));
    // 1. Fetch the URI assigned to the CURRENT environment (Dev, UAT, or Prod)
    var keyVaultUri = builder.Configuration["KeyVaultUri"];
    
    // 2. Only attempt to connect to the cloud vault if a URI is actually provided
    if (!string.IsNullOrEmpty(keyVaultUri))
    {
        try
        {
            builder.Configuration.AddAzureKeyVault(
                new Uri(keyVaultUri),
                new DefaultAzureCredential());
        }
        catch (Exception ex)
        {
            // Edge Case Handling: If Azure is unreachable during startup, 
            // log the critical failure so Application Insights can capture it.
            Console.WriteLine($"Critical Auth Failure: Unable to connect to Key Vault. {ex.Message}");
        }
    }
    
    // 1. Fetch the connection string. 
    // Because of your Key Vault setup, this will automatically pull the secure string from Azure!
    var connectionString = builder.Configuration.GetConnectionString("IdentityDb");
    
    // 2. Configure Entity Framework
    builder.Services.AddDbContext<AppIdentityDbContext>(options =>
        options.UseSqlServer(connectionString, sqlOptions =>
        {
            // enable retry on transient failures
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 2,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
    
            // increase command timeout if needed
            sqlOptions.CommandTimeout(180);
        }));
    
    builder.Services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<AppIdentityDbContext>()
        .AddDefaultTokenProviders();
    
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();
    builder.Services.AddControllers();
    builder.Services.AddSwaggerGen();
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowReactApp", policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });
    
    var app = builder.Build();
    var serviceScope=app.Services.CreateScope();
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
    await dbContext.Database.MigrateAsync();
    await DbSeeder.SeedRolesAndAdminAsync(serviceScope.ServiceProvider);
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }
    
    app.UseHttpsRedirection();
    app.UseCors("AllowReactApp");
    
    app.UseAuthorization();
    app.MapControllers();
    app.UseSwagger();
    app.UseSwaggerUI();
    
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Critical host startup failure encountered.");
}
finally
{
    Log.CloseAndFlush();
}