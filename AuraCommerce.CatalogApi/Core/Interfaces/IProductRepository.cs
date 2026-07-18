using AuraCommerce.CatalogApi.Core.DTOs;

namespace AuraCommerce.CatalogApi.Core.Interfaces
{
    public interface IProductRepository
    {
        public Task<List<ProductDto>> GetAllProducts();
        public Task<ProductDto> GetProduct(int productId);
        public Task<ProductDto> CreateProduct(CreateProductDto product);
        public Task<bool> UpdateProduct(int id, UpdateProductDto productDto);
        public Task<bool> DeleteProduct(int id);
    }
}
