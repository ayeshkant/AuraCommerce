using AuraCommerce.Orders.Domain.Entities;
using AuraCommerce.Orders.Domain.Interfaces;
using AuraCommerce.Orders.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.Orders.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _dbContext;

        public OrderRepository(OrderDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(Order order)
        {
            await _dbContext.Orders.AddAsync(order);
        }

        public async Task<IReadOnlyList<Order>> GetByCustomerIdAsync(string customerId)
        {
            return await _dbContext.Orders.Include(o => o.Items)
                                .Where(o => o.CustomerId== customerId).ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order?> GetByIdempotencyKeyAsync(string idempotencyKey)
        {
            return await _dbContext.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.IdempotencyKey == idempotencyKey);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
