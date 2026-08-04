using AuraCommerce.Orders.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.Orders.Domain.Entities
{
    public class OrderPlacedDomainEvent : IDomainEvent
    {
        public OrderPlacedDomainEvent(Guid OrderId, string CustomerId, List<OrderLineItemSnapshot> items, DateTime CreatedDate,OrderStatus orderStatus,decimal TotalAmount)
        {
            this.OrderId = OrderId;
            this.CustomerId = CustomerId;
            this.TotalAmount = TotalAmount;
            Items = items;
            Status = orderStatus;
            OccurredOn = CreatedDate;
        }
        public Guid OrderId { get; }
        public string CustomerId { get; }
        public List<OrderLineItemSnapshot> Items { get; }
        public OrderStatus Status { get; }
        public decimal TotalAmount { get; }
        public DateTime OccurredOn { get; }
    }
}
