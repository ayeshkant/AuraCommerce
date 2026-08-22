using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.Orders.Application.Interfaces
{
    public interface ICatalogServiceClient
    {
        Task<ProductInfo?> GetProductAsync(string productSku);
    }

    public record ProductInfo(string ProductSku, string Name, decimal Price);
}
