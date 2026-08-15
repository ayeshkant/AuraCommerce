using AuraCommerce.Orders.Application.DTOs;
using AuraCommerce.Orders.Application.Interfaces;
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
            // Step 2 (Catalog resolution) and Step 3 (Order.Create + persist) go here next

            throw new NotImplementedException();
        }
    }
}
