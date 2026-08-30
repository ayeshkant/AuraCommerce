using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuraCommerce.Orders.Application.Interfaces
{
    public interface ICatalogServiceClient
    {
        Task<ProductInfo?> GetProductAsync(string productSku);
    }

    public record ProductInfo(
        [property: JsonPropertyName("Sku")] string ProductSku,
        [property: JsonPropertyName("ProductName")] string Name, 
        decimal Price);
}
