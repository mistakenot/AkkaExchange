using System;
using System.Collections.Immutable;
using AkkaExchange.Orders.Events;

namespace AkkaExchange.Orders
{
    public class OrderBookState : IState<OrderBookState>
    {
        public IImmutableList<PlacedOrder> PendingOrders { get; }
        public IImmutableList<PlacedOrder> ExecutingOrders { get; }

        public OrderBookState(
            IImmutableList<PlacedOrder> pendingOrders, 
            IImmutableList<PlacedOrder> executingOrders)
        {
            ExecutingOrders = executingOrders ?? throw new ArgumentNullException(nameof(executingOrders));
            PendingOrders = pendingOrders ?? throw new ArgumentNullException(nameof(pendingOrders));
        }

        public OrderBookState Update(IEvent evnt)
        {
            if (evnt is NewOrderEvent newOrderEvent)
            {
                return new OrderBookState(
                    PendingOrders.Add(newOrderEvent.Order),
                    ExecutingOrders);
            }

            if (evnt is AmendOrderEvent amendOrderEvent)
            {
                return new OrderBookState(
                    PendingOrders
                        .RemoveAll(o => o.Details.OrderId == amendOrderEvent.Order.Details.OrderId)
                        .Add(amendOrderEvent.Order),
                    ExecutingOrders);
            }

            if (evnt is MatchedOrdersEvent matchedOrdersEvent)
            {
                
            }

            return this;
        }

        public static readonly OrderBookState Empty = 
            new OrderBookState(
                ImmutableList<PlacedOrder>.Empty,
                ImmutableList<PlacedOrder>.Empty);
    }
}
