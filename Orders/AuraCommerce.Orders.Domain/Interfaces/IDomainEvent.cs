using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.Orders.Domain.Interfaces
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
