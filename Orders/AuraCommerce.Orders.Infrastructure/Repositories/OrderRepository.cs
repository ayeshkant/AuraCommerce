using AuraCommerce.Orders.Domain.Entities;
using AuraCommerce.Orders.Domain.Exceptions;
using AuraCommerce.Orders.Domain.Interfaces;
using AuraCommerce.Orders.Infrastructure.Configurations;
using AuraCommerce.Orders.Infrastructure.Context;
using Microsoft.Data.SqlClient;
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
            return await _dbContext.Orders.AsNoTracking()
                .Include(o => o.Items)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order?> GetByIdempotencyKeyAsync(string customerId, string idempotencyKey)
        {
            return await _dbContext.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.CustomerId == customerId && o.IdempotencyKey == idempotencyKey);
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new OrderConcurrencyException("The order was modified by another request.", ex);
            }
            catch (DbUpdateException ex) when (IsIdempotencyViolation(ex))
            {
                throw new DuplicateOrderException("An order with this idempotency key already exists.", ex);
            }
        }
        private static bool IsIdempotencyViolation(DbUpdateException ex) =>
            ex.InnerException is SqlException { Number: 2601 or 2627 } sql
            && sql.Message.Contains(OrderConfiguration.IdempotencyIndexName);
    }
}
