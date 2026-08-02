using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.Orders.Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
        public OrderItem(string productId, string productName, decimal unitPrice, int quantity)
        {
            // validation here
            if (string.IsNullOrEmpty(productId))
            {
                throw new ArgumentException("Product Id is empty or null", nameof(productId));
            }
            if (string.IsNullOrEmpty(productName))
            {
                throw new ArgumentException("Product Name cannot be empty or null", nameof(productName));
            }
            if (unitPrice<0)
            {
                throw new ArgumentException("Unit Price cannot be negative", nameof(unitPrice));
            }
            if (quantity < 0)
            {
                throw new ArgumentException("Quantity cannot be negative", nameof(quantity));
            }
            // assignment here
            Id = Guid.NewGuid();
            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }
        private OrderItem()
        {

        }
    }
}
