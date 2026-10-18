using AuraCommerce.Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.Orders.Infrastructure.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public const string IdempotencyIndexName = "IX_Orders_CustomerId_IdempotencyKey";
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).ValueGeneratedNever();

            builder.Property(o => o.CustomerId).IsRequired().HasMaxLength(450);
            builder.Property(o => o.IdempotencyKey).IsRequired().HasMaxLength(100);
            builder.Property(o => o.TotalAmount).HasPrecision(18, 2);
            builder.Property(o => o.CreatedDate).IsRequired();
            builder.Property(o => o.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
            builder.Property<byte[]>("RowVersion").IsRowVersion();

            builder.Ignore(o => o.DomainEvents);

            builder.HasMany(o => o.Items).WithOne()
                .HasForeignKey("OrderId").IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(o => o.Items)
                .HasField("_items").UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(o => new { o.CustomerId, o.IdempotencyKey })
                .IsUnique().HasDatabaseName(IdempotencyIndexName);
            builder.HasIndex(o => new { o.CustomerId, o.CreatedDate });
        }
    }
}
