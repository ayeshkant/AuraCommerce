using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.Orders.Domain.Entities
{
    public enum ErrorType
    {
        None,
        Validation,
        NotFound,
        Conflict
    }
}
