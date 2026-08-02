using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.Orders.Domain.Entities
{
    public class Result
    {
        public bool IsSuccess { get;}
        public string? Error { get;}
        protected Result(bool isSuccess, string? error)
        {
            if (!isSuccess && string.IsNullOrEmpty(error))
            {
                throw new InvalidOperationException("A failed result must have an error message.");
            }
            if (isSuccess && !string.IsNullOrEmpty(error))
            {
                throw new InvalidOperationException("A successful result cannot have an error message.");
            }
            IsSuccess = isSuccess;
            Error = error;
        }
        public static Result Success() => new Result(true, null);
        public static Result Failure(string error) => new Result(false, error);
    }
}
