
using AuraCommerce.CatalogApi.Core.DTOs;
using AuraCommerce.CatalogApi.Core.Entities;
using AuraCommerce.CatalogApi.Core.Interfaces;
using AuraCommerce.CatalogApi.Core.Validators;
using AuraCommerce.CatalogApi.Infrastructure.Data;
using AuraCommerce.CatalogApi.Infrastructure.Middleware;
using AuraCommerce.CatalogApi.Infrastructure.Repositories;
using Azure.Core;
using Azure.Identity;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using StackExchange.Redis;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Initializing AuraCommerce Microservice host...");
    var builder=WebApplication.CreateBuilder(args);
    // 2. Wire up the Runtime Configuration Logger
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithProcessId()
        .Enrich.WithProperty("MicroserviceContext", builder.Environment.ApplicationName));
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
    var connectionString = builder.Configuration.GetConnectionString("CatalogDb");
    var redisEndPoint = builder.Configuration.GetConnectionString("Redis");
    
    if (!string.IsNullOrEmpty(redisEndPoint))
    {
        // 1. Parse the endpoint configuration (this no longer contains a password)
        var redisOptions = ConfigurationOptions.Parse(redisEndPoint);
        var tenantId = builder.Configuration["TenantId"];
    
        // 2. Attach the Microsoft Entra ID Token Credential seamlessly
        // This reuses the same identity context you are already using for Key Vault
        await redisOptions.ConfigureForAzureWithTokenCredentialAsync(new DefaultAzureCredential(new DefaultAzureCredentialOptions
        {
            TenantId = tenantId // or use AdditionallyAllowedTenants = { "*" } for broader allow
        }));
    
        // 3. Establish the connection multiplexer
        var multiplexer = await ConnectionMultiplexer.ConnectAsync(redisOptions);
    
        // 4. Register the multiplexer in case you need direct IDatabase access later in your repositories
        builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);
    
        // 5. Configure the distributed cache to use our pre-authenticated multiplexer
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            // Instead of passing a string to options.Configuration, we pass the factory
            options.ConnectionMultiplexerFactory = () => Task.FromResult<IConnectionMultiplexer>(multiplexer);
            options.InstanceName = "CatalogData_";
        });
    }
    else
    {
        builder.Services.AddDistributedMemoryCache();
    }
    
    // 2. Configure Entity Framework
    builder.Services.AddDbContext<AppDbContext>(options =>
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
    
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<IValidator<CreateProductDto>,CreateProductDtoValidator>();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        // 1. Define the Security Scheme
        options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter your token in the text input below.\r\n\r\nExample: \"eyJhbGciOiJIUzI1Ni...\"",
            Name = "Authorization",
            In = Microsoft.OpenApi.ParameterLocation.Header,
            Type = Microsoft.OpenApi.SecuritySchemeType.Http,
            Scheme = "bearer", // MUST BE STRICTLY LOWERCASE IN .NET 10
            BearerFormat = "JWT"
        });
    
        // 2. Apply the Security Requirement using the v10 Delegate syntax
        options.AddSecurityRequirement(document => new Microsoft.OpenApi.OpenApiSecurityRequirement
        {
            [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
        });
    });
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();
    builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>("SQL_Database_Check")
        // Use the factory delegate (sp = ServiceProvider) to resolve the multiplexer safely at runtime
        .AddRedis(sp => sp.GetRequiredService<IConnectionMultiplexer>(), "Redis_Cache_Check");
    builder.Services.AddAuthentication("Bearer")
        .AddJwtBearer(options =>
        {
            var secretKey = builder.Configuration["JwtSettings:SecretKey"];
            options.TokenValidationParameters=new TokenValidationParameters
            {
                ValidateIssuer=true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                ValidAudience = builder.Configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
            };
        });
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    });
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowReactApp", policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });
    var app=builder.Build();
    app.UseSerilogRequestLogging();
    
    app.UseExceptionHandler();
    app.UseCors("AllowReactApp");
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseSwagger();
    app.UseSwaggerUI();
    
    app.MapHealthChecks("/health");
    app.MapGet("/api/products", async (IProductRepository productRepository,IDistributedCache cache) =>
    {
        string cacheKey = "all_products";
        var cachedData = await cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            var cachedProducts = JsonSerializer.Deserialize<List<ProductDto>>(cachedData);
            return Results.Ok(cachedProducts);
        }
    
        // Write your async LINQ query here to fetch products
        var products=await productRepository.GetAllProducts();
    
        var cacheOptions = new DistributedCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromHours(1)) // Hard kill switch after 1 hour
        .SetSlidingExpiration(TimeSpan.FromMinutes(10)); // Keep alive if requested within 10 min
        var serializedData=JsonSerializer.Serialize(products);
        await cache.SetStringAsync(cacheKey, serializedData, cacheOptions);
    
        // Return the list!
        return Results.Ok(products);
    });
    app.MapGet("/api/products/{id}", async (int id,IProductRepository productRepository, IDistributedCache cache) =>
    {
        string cacheKey = $"product_{id}";
        var cachedData = await cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            var cachedProduct=JsonSerializer.Deserialize<ProductDto>(cachedData);
            return Results.Ok(cachedProduct);
        }
        // Write your async LINQ query here to fetch products
        var product = await productRepository.GetProduct(id);
        if (product == null) return Results.NotFound();
        var serializedData = JsonSerializer.Serialize(product);
        var cacheOption=new DistributedCacheEntryOptions()
                            .SetAbsoluteExpiration(TimeSpan.FromHours(1)) // Hard kill switch after 1 hour
                            .SetSlidingExpiration(TimeSpan.FromMinutes(10)); // Keep alive if requested within 10 min
        await cache.SetStringAsync(cacheKey, serializedData, cacheOption);
        return Results.Ok(product);
    });
    app.MapPost("/api/products", async (CreateProductDto newProduct, IProductRepository productRepository, IValidator<CreateProductDto> validator, IDistributedCache cache) =>
    {
        // Write your async LINQ query here to fetch products
        var validationResult=await validator.ValidateAsync(newProduct);
        if (validationResult.IsValid)
        {
            var product = await productRepository.CreateProduct(newProduct);
            // 2. INVALIDATE THE CACHE
            await cache.RemoveAsync("all_products");
            var serializedData = JsonSerializer.Serialize(product);
            var cacheOption = new DistributedCacheEntryOptions()
                                .SetAbsoluteExpiration(TimeSpan.FromHours(1))
                                .SetSlidingExpiration(TimeSpan.FromMinutes(10));
            await cache.SetStringAsync($"product_{product.Id}", serializedData, cacheOption);
            return Results.Created($"/api/products/{product.Id}", product);
        }
        return Results.ValidationProblem(validationResult.ToDictionary());
    }).RequireAuthorization("RequireAdminRole"); //RequireAuthorization("RequireAdminRole")
    app.MapPatch("/api/products/{id}", async (int id,UpdateProductDto productDto, IProductRepository productRepository, IDistributedCache cache) =>
    {
        var updated = await productRepository.UpdateProduct(id, productDto);
        if (!updated) return Results.NotFound();
    
        // Cache Invalidation is mandatory after an update!
        await cache.RemoveAsync($"product_{id}");
        await cache.RemoveAsync("all_products");
    
        return Results.NoContent(); // 204 No Content is standard for successful updates
    }).RequireAuthorization("RequireAdminRole");
    app.MapDelete("/api/products/{id}", async (int id, IProductRepository productRepository, IDistributedCache cache) =>
    {
        var deleted = await productRepository.DeleteProduct(id);
        if (!deleted) return Results.NotFound();
    
        // Cache Invalidation is mandatory after an update!
        await cache.RemoveAsync($"product_{id}");
        await cache.RemoveAsync("all_products");
    
        return Results.NoContent(); // 204 No Content is standard for successful updates
    }).RequireAuthorization("RequireAdminRole");
    
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