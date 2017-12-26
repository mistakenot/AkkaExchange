using AkkaExchange.Orders;
using System;
using System.Collections.Immutable;

namespace AkkaExchange.Matching.Events
{
    public class BeginMatchOrdersEvent : IEvent
    {
        public IImmutableList<Order> Orders { get; }

        public BeginMatchOrdersEvent(IImmutableList<Order> orders)
        {
            Orders = orders ?? throw new ArgumentNullException(nameof(orders));
        }
    }
}