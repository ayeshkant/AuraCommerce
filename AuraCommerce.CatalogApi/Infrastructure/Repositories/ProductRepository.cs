using AuraCommerce.CatalogApi.Core.DTOs;
using AuraCommerce.CatalogApi.Core.Entities;
using AuraCommerce.CatalogApi.Core.Interfaces;
using AuraCommerce.CatalogApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuraCommerce.CatalogApi.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _dbContext;

        public ProductRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ProductDto> CreateProduct(CreateProductDto product)
        {
            var newProduct = new Product { Name = product.Name, SKU = product.SKU, Price = product.Price, CategoryId = product.CategoryId };
            // 2. Track it
            _dbContext.Products.Add(newProduct);

            // 3. SAVE IT to SQL Server (This generates the new ID)
            await _dbContext.SaveChangesAsync();

            // 4. Map the saved entity back to the Output DTO
            return new ProductDto
            {
                ProductName = newProduct.Name,
                Sku = newProduct.SKU,
                Id = newProduct.Id
            };
        }

        public async Task<List<ProductDto>> GetAllProducts()
        {
            var products = await _dbContext.Products.Select(p => new ProductDto { ProductName = p.Name, Sku = p.SKU }).ToListAsync();

            return products;
        }

        public async Task<ProductDto> GetProduct(int productId)
        {
            var product = await _dbContext.Products.Where(p => p.Id == productId).Select(p => new ProductDto { ProductName = p.Name, Sku = p.SKU }).FirstOrDefaultAsync();

            return product;
        }
        public async Task<bool> UpdateProduct(int id, UpdateProductDto productDto)
        {
            // 1. Create a stub entity with just the primary key
            var productStub = new Product { Id = id };
            // 2. Attach it to the context. EF now tracks it, but thinks it is completely unmodified.
            _dbContext.Products.Attach(productStub);

            int rowsAffected = 0;

            // 3. SAVE IT to SQL Server (This generates the new ID)
            if (productDto.Price.HasValue)
            {
                productStub.Price = productDto.Price.Value;
                _dbContext.Entry(productStub).Property(p => p.Price).IsModified = true;
            }
            if (!string.IsNullOrEmpty(productDto.Name))
            {
                productStub.Name = productDto.Name;
                _dbContext.Entry(productStub).Property(p => p.Name).IsModified = true;
            }
            if (!string.IsNullOrEmpty(productDto.SKU))
            {
                productStub.SKU = productDto.SKU;
                _dbContext.Entry(productStub).Property(p => p.SKU).IsModified = true;
            }
            if (productDto.CategoryId.HasValue)
            {
                productStub.CategoryId = productDto.CategoryId.Value;
                _dbContext.Entry(productStub).Property(p => p.CategoryId).IsModified = true;
            }
            // 4. Save changes. EF Core translates this into a single SQL UPDATE query!
            rowsAffected = await _dbContext.SaveChangesAsync();

            return rowsAffected > 0;
        }
        public async Task<bool> DeleteProduct(int id)
        {
            int rowsAffected =await _dbContext.Products.Where(p => p.Id == id).ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDeleted, true));

            return rowsAffected > 0;
        }
    }
}
