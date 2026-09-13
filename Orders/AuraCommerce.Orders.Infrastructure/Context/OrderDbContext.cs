using AuraCommerce.Orders.Domain.Entities;
using AuraCommerce.Orders.Domain.Interfaces;
using AuraCommerce.Orders.Infrastructure.Event;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AuraCommerce.Orders.Infrastructure.Context
{
    public class OrderDbContext:DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
            
        }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Order>(builder =>
            {
                builder.HasMany(o => o.Items)
                    .WithOne()
                    .HasForeignKey("OrderId")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OrderItem>(builder =>
            {
                builder.HasKey(oi => oi.Id);
            });
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var aggregatesWithEvents = ChangeTracker.Entries<AggregateRoot>()
                .Select(a => a.Entity)
                .Where(e => e.DomainEvents.Any())
                .ToList();

            foreach (var aggregate in aggregatesWithEvents)
            {
                foreach (var domainEvent in aggregate.DomainEvents)
                {
                    var outboxMessage = new OutboxMessage(
                        domainEvent.GetType().Name,
                        JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
                        domainEvent.OccurredOn);

                    OutboxMessages.Add(outboxMessage);
                }
                aggregate.ClearDomainEvents();
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
