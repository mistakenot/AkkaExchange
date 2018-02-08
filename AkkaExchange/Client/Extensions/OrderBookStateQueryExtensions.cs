using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using AkkaExchange.Orders;
using AkkaExchange.Orders.Events;

namespace AkkaExchange.Client.Extensions
{
    public static class OrderBookStateQueryExtensions
    {
        public static IObservable<OpenOrders> OpenOrders(this IObservable<OrderBookState> stateQuery, Guid clientId)
            => stateQuery.Select(s => 
                new OpenOrders(
                    s.OpenOrders.Where(o => o.ClientId == clientId).ToHashSet()));

        public static IObservable<CompleteOrders> CompletedOrders(this IObservable<CompleteOrderEvent> events)
            => events.Scan(
                new CompleteOrders(100),
                (state, e) => state.Add(e.Order));
    }

    public class CompleteOrders : Message
    {
        public IImmutableList<PlacedOrder> Orders { get; }
        public int Limit { get; }

        public CompleteOrders(int limit)
            : this(ImmutableList<PlacedOrder>.Empty, limit)
        {
            
        }

        public CompleteOrders(IImmutableList<PlacedOrder> orders, int limit)
        {
            Orders = orders;
            Limit = limit;
        }

        public CompleteOrders Add(PlacedOrder order)
        {
            return new CompleteOrders(
                Orders.Add(order).OrderByDescending(o => o.PlacedAt).Take(Limit).ToImmutableList(),
                Limit);
        }
    }
    public class OpenOrders : Message
    {
        public ISet<PlacedOrder> Orders { get; }

        public OpenOrders(ISet<PlacedOrder> orders)
        {
            Orders = orders;
        }

        public override int GetHashCode() => 
            Orders.Aggregate(92821, (a, o) =>
            {
                unchecked
                {
                    return a ^ o.GetHashCode();
                }
            });

        public override bool Equals(object obj) =>
            obj is OpenOrders && (obj as OpenOrders).GetHashCode() == GetHashCode();
    }
}