using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.Orders.Domain.Interfaces
{
    public abstract class AggregateRoot
    {
        public readonly List<IDomainEvent> _domainEvents = new();
        IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
        protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
