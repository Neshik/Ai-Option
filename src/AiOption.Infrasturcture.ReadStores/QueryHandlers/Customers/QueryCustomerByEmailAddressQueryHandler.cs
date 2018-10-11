﻿using System.Threading;
using System.Threading.Tasks;
using AiOption.Domain.Customers;
using AiOption.Query;
using AiOption.Query.Customers;
using EventFlow.EntityFramework;
using EventFlow.Exceptions;
using EventFlow.Queries;
using Microsoft.EntityFrameworkCore;

namespace AiOption.Infrasturcture.ReadStores.QueryHandlers.Customers
{
    internal class QueryCustomerByEmailAddressQueryHandler : IQueryHandler<QueryCustomerByEmailAddress, Customer>
    {
        private readonly ISearchableReadModelStore<CustomerReadModel> _readStore;
        private readonly IDbContextProvider<AiOptionDbContext> _dbContextProvider;

        public QueryCustomerByEmailAddressQueryHandler(ISearchableReadModelStore<CustomerReadModel> readStore, IDbContextProvider<AiOptionDbContext> dbContextProvider)
        {
            _readStore = readStore;
            _dbContextProvider = dbContextProvider;
        }

        public async Task<Customer> ExecuteQueryAsync(QueryCustomerByEmailAddress query,
            CancellationToken cancellationToken)
        {
            using (var db = _dbContextProvider.CreateContext())
            {
                var entity = await db.Customers
                    .FirstOrDefaultAsync(x => x.EmailAddressNormalize == query.User.Value,
                        cancellationToken);

                if (query.ThrowIfNotFound && entity == null) throw DomainError.With($"{query.User} not found!");

                return entity?.ToCustomer();
            }
        }
    }
}