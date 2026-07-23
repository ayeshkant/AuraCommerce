using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.Orders.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public string CustomerId { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public decimal TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public OrderStatus Status { get; private set; }
        public string IdempotencyKey { get; set; }

        
    }
}
