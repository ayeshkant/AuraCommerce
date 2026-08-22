using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.Orders.Application.DTOs
{
    public record CreateOrderRequest(string CustomerId, List<CreateOrderItemRequest> Items, string IdempotencyKey);
    public record CreateOrderItemRequest(string ProductSku, int Quantity);
}
