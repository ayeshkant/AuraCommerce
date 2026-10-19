using AuraCommerce.Orders.Domain.Entities;
using AuraCommerce.Orders.Infrastructure.Event;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.Orders.Infrastructure.Configurations
{
    public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("OutboxMessages");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).ValueGeneratedNever();
            builder.Property(m => m.EventType).IsRequired().HasMaxLength(200);
            builder.Property(m => m.Payload).IsRequired();
            builder.Property(m => m.OccurredOn).IsRequired();
            builder.Property(m => m.ProcessedOn);

            builder.HasIndex(m => m.OccurredOn)
                .HasDatabaseName("IX_OutboxMessages_Unprocessed")
                .HasFilter("[ProcessedOn] IS NULL");
        }
    }
}
