namespace AuraCommerce.CatalogApi.Core.DTOs
{
    public class UpdateProductDto
    {
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public string? SKU { get; set; }
        public int? CategoryId { get; set; }
    }
}
