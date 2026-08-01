using AuraCommerce.Orders.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.Orders.Domain.Entities
{
    public class OrderPlacedDomainEvent : IDomainEvent
    {
        public OrderPlacedDomainEvent(Guid OrderId, string CustomerId, List<OrderItem> items, DateTime CreatedDate,OrderStatus orderStatus,decimal TotalAmount)
        {
            //OccurredOn = CreatedDate;
        }
        public DateTime OccurredOn => throw new NotImplementedException();
    }
}
