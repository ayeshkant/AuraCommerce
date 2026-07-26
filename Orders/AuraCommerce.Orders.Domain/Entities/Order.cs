using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace AuraCommerce.Orders.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; private set; }
        public string CustomerId { get; private set; }
        public List<OrderItem> Items { get; private set; } = new List<OrderItem>();
        public decimal TotalAmount { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public OrderStatus Status { get; private set; }
        public string IdempotencyKey { get; private set; }

        private Order()
        {
            
        }
        static void Create()
        {

        }
    }
}
