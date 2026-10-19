using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.Orders.Domain.Exceptions
{
    public class DuplicateOrderException: Exception
    {
        public DuplicateOrderException(string message, Exception? innerException = null) : base(message, innerException)
        {
        }
    }
}
