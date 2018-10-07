﻿using AiOption.Domain.Common;
using AiOption.Domain.Customers;
using EventFlow.Queries;

namespace AiOption.Query.Customers
{
    public class QueryCustomerByEmailAddress : IQuery<Customer>
    {
        public QueryCustomerByEmailAddress(User user, bool throwIfNotFound = true)
        {
            User = user;
            ThrowIfNotFound = throwIfNotFound;
        }

        public User User { get; }
        public bool ThrowIfNotFound { get; }
    }
}