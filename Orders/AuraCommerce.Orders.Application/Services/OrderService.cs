using AuraCommerce.Orders.Application.DTOs;
using AuraCommerce.Orders.Application.Interfaces;
using AuraCommerce.Orders.Domain.Entities;
using AuraCommerce.Orders.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.Orders.Application.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICatalogServiceClient _catalogServiceClient;

        public OrderService(IOrderRepository orderRepository,ICatalogServiceClient catalogServiceClient)
        {
            _orderRepository = orderRepository;
            _catalogServiceClient = catalogServiceClient;
        }
        public async Task<Result<Guid>> CreateOrderAsync(CreateOrderRequest createOrderRequest)
        {
            var existingOrder = await _orderRepository.GetByIdempotencyKeyAsync(createOrderRequest.IdempotencyKey);

            if (existingOrder!=null)
            {
                return Result<Guid>.Success(existingOrder.Id);
            }
            // Step 2 (Catalog resolution) 
            var orderItems = new List<OrderItem>();

            foreach (var item in createOrderRequest.Items)
            {
                var product = await _catalogServiceClient.GetProductAsync(item.ProductId);
                if (product==null)
                {
                    return Result<Guid>.Failure($"Product {item.ProductId} does not exist");
                }
                orderItems.Add(new OrderItem(product.ProductId, product.Name, product.Price, item.Quantity));
            }
            // Step 3 (Order.Create + persist) goes here next
            var order = Order.Create(createOrderRequest.CustomerId, orderItems, createOrderRequest.IdempotencyKey);

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();

            return Result<Guid>.Success(order.Id);
        }
    }
}
