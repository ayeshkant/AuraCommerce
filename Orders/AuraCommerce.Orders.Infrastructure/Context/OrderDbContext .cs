using AuraCommerce.Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.Orders.Infrastructure.Context
{
    public class OrderDbContext:DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
            
        }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Order>(builder =>
            {
                // relationship + backing field configuration comes next
            });

            modelBuilder.Entity<OrderItem>(builder =>
            {
                // configuration comes next
            });
        }
    }
}
