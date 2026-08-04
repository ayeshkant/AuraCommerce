using AuraCommerce.Orders.Domain.Interfaces;

namespace AuraCommerce.Orders.Domain.Entities
{
    public class Order: AggregateRoot
    {
        public Guid Id { get; private set; }
        public string CustomerId { get; private set; }
        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
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
                throw new ArgumentException("Order Items cannot be empty or null", nameof(items));
            }
            if (string.IsNullOrEmpty(idempotencyKey))
            {
                throw new ArgumentException("Idempotency Key is empty or null", nameof(idempotencyKey));
            }

            var order = new Order();
            order.Id = Guid.NewGuid(); 
            order.CustomerId = customerId; 
            order._items.AddRange(items); 
            order.TotalAmount = items.Sum(i => i.UnitPrice * i.Quantity); 
            order.CreatedDate = DateTime.UtcNow; 
            order.Status = OrderStatus.Created; 
            order.IdempotencyKey = idempotencyKey;

            order.AddDomainEvent(
                new OrderPlacedDomainEvent(
                    order.Id, order.CustomerId, 
                    order.Items.Select(i=>new OrderLineItemSnapshot (
                        i.ProductId,i.ProductName,i.UnitPrice,i.Quantity)).ToList(), 
                    order.CreatedDate, order.Status, order.TotalAmount
                    )
                );

            return order;
        }
        public Result BeginPaymentProcessing()
        {
            if (Status != OrderStatus.Created)
            {
                return Result.Failure($"Cannot mark as payment pending from status {Status}.");
            }

            Status = OrderStatus.PaymentPending;
            return Result.Success();
        }
        public Result MarkAsPaid()
        {
            if (Status != OrderStatus.PaymentPending)
            {
                return Result.Failure($"Cannot mark as paid from status {Status}.");
            }

            Status = OrderStatus.Paid;
            return Result.Success();
        }
        public Result MarkAsShipped()
        {
            if (Status != OrderStatus.Paid)
            {
                return Result.Failure($"Cannot mark as shipped from status {Status}.");
            }

            Status = OrderStatus.Shipped;
            return Result.Success();
        }
        public Result Cancel()
        {
            if (Status != OrderStatus.Created && Status != OrderStatus.PaymentPending && Status != OrderStatus.Paid)
            {
                return Result.Failure($"Cannot mark as cancelled from status {Status}.");
            }

            Status = OrderStatus.Cancelled;
            AddDomainEvent(
            new OrderCancelledDomainEvent(
                Id, CustomerId,
                Items.Select(i => new OrderLineItemSnapshot(
                    i.ProductId, i.ProductName, i.UnitPrice, i.Quantity)).ToList(),
                DateTime.UtcNow, Status, TotalAmount
                )
            );
            return Result.Success();
        }
    }
}
