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
                throw new ArgumentException("Order Items cannot be empty or null", nameof(items));
            }

            var order = new Order();
            order.Id = Guid.NewGuid(); 
            order.CustomerId = customerId; 
            order.Items = items; 
            order.TotalAmount = items.Sum(i => i.UnitPrice * i.Quantity); 
            order.CreatedDate = DateTime.UtcNow; 
            order.Status = OrderStatus.Created; 
            order.IdempotencyKey = idempotencyKey;

            order.AddDomainEvent(
                new OrderPlacedDomainEvent(
                    order.Id, order.CustomerId, 
                    order.Items.Select(i=>new OrderPlacedItem (
                        i.ProductId,i.ProductName,i.UnitPrice,i.Quantity)).ToList(), 
                    order.CreatedDate, order.Status, order.TotalAmount
                    )
                );

            return order;
        }
        public static Result BeginPaymentProcessing(Order order)
        {
            if (order.Status == OrderStatus.Shipped || order.Status == OrderStatus.Cancelled || order.Status==OrderStatus.Paid)
                return Result.Failure("Incorrect Order Status");
            if (order.Status == OrderStatus.Created)
            {
                order.Status = OrderStatus.PaymentPending;
            }

            return Result.Success();
        }
        public static Result MarkAsPaid(Order order)
        {
            if (order.Status == OrderStatus.Shipped || order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Created)
                return Result.Failure("Incorrect Order Status");
            if (order.Status == OrderStatus.PaymentPending)
            {
                order.Status = OrderStatus.Paid;
            }

            return Result.Success();
        }
        public static Result MarkAsShipped(Order order)
        {
            if (order.Status == OrderStatus.PaymentPending || order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Created)
                return Result.Failure("Incorrect Order Status");
            if (order.Status == OrderStatus.Paid)
            {
                order.Status = OrderStatus.Shipped;
            }

            return Result.Success();
        }
        public static Result Cancel(Order order)
        {
            if (order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Shipped)
                return Result.Failure("Incorrect Order Status");
            if (order.Status == OrderStatus.Created || order.Status == OrderStatus.PaymentPending || order.Status == OrderStatus.Paid)
            {
                order.Status = OrderStatus.Cancelled;

                order.AddDomainEvent(
                new OrderCancelledDomainEvent(
                    order.Id, order.CustomerId,
                    order.Items.Select(i => new OrderPlacedItem(
                        i.ProductId, i.ProductName, i.UnitPrice, i.Quantity)).ToList(),
                    DateTime.Now, order.Status, order.TotalAmount
                    )
                );
            }

            return Result.Success();
        }
    }
}
