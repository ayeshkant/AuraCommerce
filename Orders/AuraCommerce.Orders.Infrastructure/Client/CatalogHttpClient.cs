using AuraCommerce.Orders.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace AuraCommerce.Orders.Infrastructure.Client
{
    public class CatalogHttpClient : ICatalogServiceClient
    {
        private readonly HttpClient _httpClient;

        public CatalogHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<ProductInfo?> GetProductAsync(string productSku)
        {
            var response = await _httpClient.GetAsync($"/api/products/by-sku/{productSku}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ProductInfo>();
        }
    }
}
