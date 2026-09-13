using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.Orders.Infrastructure.Event
{
    public class OutboxMessage
    {
        public Guid Id { get; private set; }
        public string EventType { get; private set; }
        public string Payload { get; private set; }
        public DateTime OccurredOn { get; private set; }
        public DateTime? ProcessedOn { get; private set; }
        public OutboxMessage(string eventType, string payload, DateTime occurredOn)
        {
            Id = Guid.NewGuid();
            EventType = eventType;
            Payload = payload;
            OccurredOn = occurredOn;
        }
        public void MarkAsProcessed()
        {
            ProcessedOn = DateTime.UtcNow;
        }
    }
}
