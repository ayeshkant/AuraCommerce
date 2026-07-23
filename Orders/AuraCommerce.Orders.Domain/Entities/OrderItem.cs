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
    }
    public enum OrderStatus
    {
        Created,
        PaymentPending, 
        Paid, 
        Shipped, 
        Cancelled
    }
}
