using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.CatalogApi.Core.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public string ProductName { get; set; }
    }
}
