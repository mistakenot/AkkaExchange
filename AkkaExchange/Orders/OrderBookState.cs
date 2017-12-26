using System;
using System.Collections.Immutable;
using AkkaExchange.Matching.Events;
using AkkaExchange.Orders.Events;

namespace AkkaExchange.Orders
{
    public class OrderBookState : IState<OrderBookState>
    {
        public IImmutableList<Order> PendingOrders { get; }
        public IImmutableList<Order> ExecutingOrders { get; }
        public bool IsMatching { get; }

        public OrderBookState(IImmutableList<Order> pendingOrders, IImmutableList<Order> executingOrders, bool isMatching)
        {
            IsMatching = isMatching;
            ExecutingOrders = executingOrders ?? throw new ArgumentNullException(nameof(executingOrders));
            PendingOrders = pendingOrders ?? throw new ArgumentNullException(nameof(pendingOrders));
        }

        public OrderBookState Update(IEvent evnt)
        {
            if (evnt is NewOrderEvent newOrderEvent)
            {
                return new OrderBookState(
                    PendingOrders.Add(newOrderEvent.Order),
                    ExecutingOrders,
                    IsMatching);
            }

            if (evnt is AmendOrderEvent amendOrderEvent)
            {
                return new OrderBookState(
                    PendingOrders
                        .RemoveAll(o => o.OrderId == amendOrderEvent.Order.OrderId)
                        .Add(amendOrderEvent.Order),
                    ExecutingOrders,
                    IsMatching);
            }

            if (evnt is BeginMatchOrdersEvent)
            {
                return new OrderBookState(
                    PendingOrders,
                    ExecutingOrders,
                    true);
            }

            if (evnt is EndMatchOrdersEvent endMatchOrdersEvent)
            {
                
            }

            return this;
        }

        public static readonly OrderBookState Empty = 
            new OrderBookState(
                ImmutableList<Order>.Empty,
                ImmutableList<Order>.Empty,
                false);
    }
}
