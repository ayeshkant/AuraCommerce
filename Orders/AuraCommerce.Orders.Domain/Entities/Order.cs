using AuraCommerce.Orders.Domain.Interfaces;

namespace AuraCommerce.Orders.Domain.Entities
{
    public class Order: AggregateRoot
    {
        public Guid Id { get; private set; }
        public string CustomerId { get; private set; }
        public List<OrderItem> Items { get; private set; }
        public decimal TotalAmount { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public OrderStatus Status { get; private set; }
        public string IdempotencyKey { get; private set; }

        private Order()
        {
            
        }
        public static Order Create(string customerId, List<OrderItem> items, string idempotencyKey)
        {
            if (string.IsNullOrEmpty(customerId))
            {
                throw new ArgumentException("Customer Id is empty or null", nameof(customerId));
            }
            if (items == null || items.Count == 0)
            {
                throw new ArgumentNullException("Order Items is empty or null", nameof(items));
            }

            var order = new Order();
            order.Id = Guid.NewGuid(); 
            order.CustomerId = customerId; 
            order.Items = items; 
            order.TotalAmount = items.Sum(i => i.UnitPrice * i.Quantity); 
            order.CreatedDate = DateTime.UtcNow; 
            order.Status = OrderStatus.Created; 
            order.IdempotencyKey = idempotencyKey;

            order.AddDomainEvent(new OrderPlacedDomainEvent(order.Id, order.CustomerId, order.Items, order.CreatedDate, order.Status, order.TotalAmount));

            return order;
        }
    }
}
