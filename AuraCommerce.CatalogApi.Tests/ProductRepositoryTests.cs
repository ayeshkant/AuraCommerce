using AuraCommerce.CatalogApi.Core.DTOs;
using AuraCommerce.CatalogApi.Core.Entities;
using AuraCommerce.CatalogApi.Infrastructure.Data;
using AuraCommerce.CatalogApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AuraCommerce.CatalogApi.Tests
{
    public class ProductRepositoryTests
    {
        [Fact]
        public async Task GetProduct_ValidId_ReturnsProduct()
        {
            var dbOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "CatalogTestDb_" + Guid.NewGuid().ToString())
                .Options;

            using var dbContext = new AppDbContext(dbOptions);

            var dummyProduct = new Product
            {
                Id = 1,
                Name = "Laptop",
                Price = 56000,
                SKU = "Lap101",
                CategoryId = 1
            };
            dbContext.Products.Add(dummyProduct);
            await dbContext.SaveChangesAsync();

            var repository = new ProductRepository(dbContext);

            var result = await repository.GetProduct(1);

            Assert.NotNull(result);
            Assert.Equal("Lap101", result.Sku);
            Assert.Equal("Laptop", result.ProductName);
        }
        [Fact]
        public async Task GetProduct_InvalidId_ReturnsNull()
        {
            var dbOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "CatalogTestDb_" + Guid.NewGuid().ToString())
                .Options;

            using var dbContext = new AppDbContext(dbOptions);

            // We seed the database with ID 1
            dbContext.Products.Add(new Product { Id = 1, Name = "Laptop", Price = 56000, SKU = "Lap101", CategoryId = 1 });
            await dbContext.SaveChangesAsync();

            var repository = new ProductRepository(dbContext);

            // --- ACT ---

            // We intentionally search for ID 99, which does not exist
            var result = await repository.GetProduct(99);

            // --- ASSERT ---

            // The repository should gracefully return null
            Assert.Null(result);
        }
        [Fact]
        public async Task Get_ValidProducts_ReturnsProduct()
        {
            var dbOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "CatalogTestDb_" + Guid.NewGuid().ToString())
                .Options;

            using var dbContext = new AppDbContext(dbOptions);

            var dummyProducts = new List<Product> {
            new Product{
                Id = 1,
                Name = "Laptop",
                Price = 56000,
                SKU = "Lap101",
                CategoryId = 1
            },new Product{
                Id = 2,
                Name = "Laptop",
                Price = 66000,
                SKU = "Lap102",
                CategoryId = 1
            }};
            dbContext.Products.AddRange(dummyProducts);
            await dbContext.SaveChangesAsync();

            var repository = new ProductRepository(dbContext);

            var result = await repository.GetAllProducts();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            foreach (var item in result)
            {
                Assert.Equal("Laptop", item.ProductName);
            }
        }
        [Fact]
        public async Task CreateProduct_ValidProduct_SavesToDatabase()
        {
            var dbOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "CatalogTestDb_" + Guid.NewGuid().ToString())
                .Options;

            using var dbContext = new AppDbContext(dbOptions);

            var repository = new ProductRepository(dbContext);

            var dummyProduct = new CreateProductDto
            {
                Name = "Laptop",
                Price = 56000,
                SKU = "Lap101",
                CategoryId = 1
            };

            var product=await repository.CreateProduct(dummyProduct);
            var productCount = dbContext.Products.Count();

            Assert.NotNull(product);
            Assert.Equal("Laptop", product.ProductName);
            Assert.Equal("Lap101", product.Sku);
            Assert.Equal(1, productCount);
        }
    }
}
