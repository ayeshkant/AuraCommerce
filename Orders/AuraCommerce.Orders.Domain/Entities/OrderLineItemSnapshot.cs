using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.Orders.Domain.Entities
{
    public sealed record OrderLineItemSnapshot(string ProductId, string ProductName, decimal UnitPrice, int Quantity);
}
