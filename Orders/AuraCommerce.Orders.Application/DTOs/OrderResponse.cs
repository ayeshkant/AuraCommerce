using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.Orders.Application.DTOs
{
    public record OrderItemResponse(string ProductSku, string ProductName, decimal UnitPrice, int Quantity, decimal LineTotal);

    public record OrderResponse(
        Guid Id,
        string CustomerId,
        string Status,
        decimal TotalAmount,
        DateTime CreatedDate,
        IReadOnlyList<OrderItemResponse> Items);
}
