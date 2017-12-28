using System;
using System.Collections.Immutable;
using System.Linq;
using AkkaExchange.Orders.Events;
using AkkaExchange.Orders.Extensions;

namespace AkkaExchange.Orders
{
    public class OrderBookState : IState<OrderBookState>
    {
        public IImmutableList<PlacedOrder> OpenOrders { get; }
        public IImmutableList<PlacedOrder> PendingOrders { get; }
        public IImmutableList<PlacedOrder> ExecutingOrders { get; }

        public OrderBookState(
            IImmutableList<PlacedOrder> openOrders, 
            IImmutableList<PlacedOrder> executingOrders, 
            IImmutableList<PlacedOrder> pendingOrders)
        {
            PendingOrders = pendingOrders ?? throw new ArgumentNullException(nameof(pendingOrders));
            ExecutingOrders = executingOrders ?? throw new ArgumentNullException(nameof(executingOrders));
            OpenOrders = openOrders ?? throw new ArgumentNullException(nameof(openOrders));
        }

        public OrderBookState Update(IEvent evnt)
        {
            if (evnt is NewOrderEvent newOrderEvent)
            {
                return new OrderBookState(
                    OpenOrders.Add(newOrderEvent.Order),
                    ExecutingOrders,
                    PendingOrders);
            }

            if (evnt is AmendOrderEvent amendOrderEvent)
            {
                return new OrderBookState(
                    OpenOrders
                        .RemoveAll(o => o.OrderId == amendOrderEvent.Order.OrderId)
                        .Add(amendOrderEvent.Order),
                    ExecutingOrders,
                    PendingOrders);
            }

            if (evnt is MatchedOrdersEvent matchedOrdersEvent)
            {
                return matchedOrdersEvent.MatchedOrders.Matches.Aggregate(
                    this,
                    (s, m) =>
                    {
                        var existingBid = s.OpenOrders.Single(o => o.OrderId == m.Bid.OrderId);
                        var existingAsk = s.OpenOrders.Single(o => o.OrderId == m.Ask.OrderId);

                        var newBid = existingBid.WithAmount(existingBid.Amount - m.Bid.Amount);
                        var newAsk = existingAsk.WithAmount(existingAsk.Amount - m.Ask.Amount);
                        var newPendingOrders = new[] {newBid, newAsk}.Where(o => o.Amount > 0);

                        return new OrderBookState(
                            OpenOrders
                                .RemoveAll(o => o.OrderId == m.Bid.OrderId || o.OrderId == m.Ask.OrderId)
                                .AddRange(newPendingOrders),
                            ExecutingOrders,
                            PendingOrders
                                .Add(m.Bid)
                                .Add(m.Ask));
                    });
            }

            return this;
        }

        public static readonly OrderBookState Empty = 
            new OrderBookState(
                ImmutableList<PlacedOrder>.Empty,
                ImmutableList<PlacedOrder>.Empty,
                ImmutableList<PlacedOrder>.Empty);
    }
}
