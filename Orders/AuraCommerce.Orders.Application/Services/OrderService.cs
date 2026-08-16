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

        public OrderService(IOrderRepository orderRepository, ICatalogServiceClient catalogServiceClient)
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
            var order = Order.Create(createOrderRequest.CustomerId, orderItems, createOrderRequest.IdempotencyKey);

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();

            return Result<Guid>.Success(order.Id);
        }
        public async Task<Result> CancelOrderAsync(Guid orderId, string customerId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order==null || order.CustomerId!=customerId)
            {
                return Result.Failure("Order not found");
            }
            var cancelResult=order.Cancel();

            if (!cancelResult.IsSuccess)
            {
                return cancelResult;
            }

            await _orderRepository.SaveChangesAsync();
            return Result.Success();
        }
        public async Task<Result> MarkOrderAsPaidAsync(Guid orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                return Result.Failure("Order not found");
            }
            var orderResult = order.MarkAsPaid();

            if (!orderResult.IsSuccess)
            {
                return orderResult;
            }

            await _orderRepository.SaveChangesAsync();
            return Result.Success();
        }
        public async Task<Result> MarkOrderAsShippedAsync(Guid orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                return Result.Failure("Order not found");
            }
            var orderResult = order.MarkAsShipped();

            if (!orderResult.IsSuccess)
            {
                return orderResult;
            }

            await _orderRepository.SaveChangesAsync();
            return Result.Success();
        }
    }
}
