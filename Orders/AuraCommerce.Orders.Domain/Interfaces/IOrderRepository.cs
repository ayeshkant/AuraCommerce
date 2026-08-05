using AuraCommerce.Orders.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.Orders.Domain.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<Order>> GetByCustomerIdAsync(string customerId);
        Task<bool> ExistsWithIdempotencyKeyAsync(string idempotencyKey);
        Task AddAsync(Order order);
        Task SaveChangesAsync();
    }
}
