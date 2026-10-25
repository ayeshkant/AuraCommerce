using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.Orders.Domain.Entities
{
    public class Result
    {
        public bool IsSuccess { get;}
        public string? Error { get;}
        public ErrorType Type { get; }
        protected Result(bool isSuccess, string? error, ErrorType type)
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
            Type = type;
        }
        public static Result Success() => new Result(true, null, ErrorType.None);
        public static Result Failure(string error, ErrorType type) => new Result(false, error, type);
    }
}
