using AuraCommerce.Orders.Application.DTOs;
using AuraCommerce.Orders.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.Orders.Application.Mappings
{
    public static class OrderMappings
    {
        public static OrderResponse ToResponse(this Order order) => new(
            order.Id,
            order.CustomerId,
            order.Status.ToString(),
            order.TotalAmount,
            order.CreatedDate,
            order.Items.Select(i => new OrderItemResponse(
                i.ProductSku, i.ProductName, i.UnitPrice, i.Quantity, i.LineTotal)).ToList());
    }
}
