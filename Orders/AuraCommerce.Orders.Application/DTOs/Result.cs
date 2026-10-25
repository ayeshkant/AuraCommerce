using AuraCommerce.Orders.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.Orders.Application.DTOs
{
    public class Result<T> : Result
    {
        public T? Value { get; }
        protected Result(bool isSuccess, T? value, string? error, ErrorType type) : base(isSuccess, error, type)
        {
            Value = value;
        }

        public static Result<T> Success(T value) => new(true, value, null, ErrorType.None);
        public new static Result<T> Failure(string error, ErrorType type) => new(false, default, error, type);
    }
}
