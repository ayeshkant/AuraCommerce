using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.Orders.Domain.Exceptions
{
    public class OrderConcurrencyException: Exception
    {
        public OrderConcurrencyException(string message, Exception? innerException = null):base(message, innerException)
        {
        }
    }
}
