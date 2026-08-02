using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.Orders.Domain.Entities
{
    public enum OrderStatus
    {
        Created,
        PaymentPending,
        Paid,
        Shipped,
        Cancelled
    }
}
