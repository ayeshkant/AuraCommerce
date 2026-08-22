using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.Orders.Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get; private set; }
        public string ProductSku { get; private set; }
        public string ProductName { get; private set; }
        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; }
        public decimal LineTotal => UnitPrice * Quantity;
        public OrderItem(string productSku, string productName, decimal unitPrice, int quantity)
        {
            // validation here
            if (string.IsNullOrEmpty(productSku))
            {
                throw new ArgumentException("Product Sku is empty or null", nameof(productSku));
            }
            if (string.IsNullOrEmpty(productName))
            {
                throw new ArgumentException("Product Name cannot be empty or null", nameof(productName));
            }
            if (unitPrice<0)
            {
                throw new ArgumentException("Unit Price cannot be negative", nameof(unitPrice));
            }
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
            }
            // assignment here
            Id = Guid.NewGuid();
            ProductSku = productSku;
            ProductName = productName;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }
        private OrderItem()
        {

        }
    }
}
