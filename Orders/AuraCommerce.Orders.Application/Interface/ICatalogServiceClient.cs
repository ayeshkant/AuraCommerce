using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.Orders.Application.Interface
{
    public interface ICatalogServiceClient
    {
        Task<ProductInfo?> GetProductAsync(string productId);
    }

    public record ProductInfo(string ProductId, string Name, decimal Price);
}
