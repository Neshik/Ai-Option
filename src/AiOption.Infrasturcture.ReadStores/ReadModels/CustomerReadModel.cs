﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using AiOption.Domain.Common;
using AiOption.Domain.Customers;
using AiOption.Domain.Customers.Events;
using AiOption.Domain.IqAccounts;
using EventFlow.Aggregates;
using EventFlow.ReadStores;

namespace AiOption.Infrasturcture.ReadStores.ReadModels
{
    [Table("Customers")]
    public class CustomerReadModelDto : CustomerReadModel
    {
        [Key] [Column("CustomerId")] public override string AggregateId { get; set; }
    }
}