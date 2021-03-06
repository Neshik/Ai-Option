﻿using System.Reflection;
using EventFlow;
using EventFlow.Extensions;

// ReSharper disable once CheckNamespace
namespace AiOption.Infrastructure.ReadStores
{
    internal static class ReadStore
    {
    }

    public static class AiOption
    {
        public static Assembly AiOptionAssembly => typeof(ReadStore).Assembly;

        public static IEventFlowOptions AddInfrastructure(this IEventFlowOptions options)
        {
            return options.AddDefaults(AiOptionAssembly);
        }
    }
}