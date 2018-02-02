using System;
using System.Collections.Immutable;
using System.Linq;
using AkkaExchange.Orders.Events;
using AkkaExchange.Orders.Extensions;

namespace AkkaExchange.Orders
{
    public class OrderBookState : Message, IState<OrderBookState>
    {
        public IImmutableList<PlacedOrder> OpenOrders { get; }
        public IImmutableList<PlacedOrder> ExecutingOrders { get; }
        public IImmutableList<PlacedOrder> CompleteOrders { get; }

        public OrderBookState(
            IImmutableList<PlacedOrder> openOrders, 
            IImmutableList<PlacedOrder> executingOrders, 
            IImmutableList<PlacedOrder> completeOrders)
        {
            OpenOrders = openOrders ?? throw new ArgumentNullException(nameof(openOrders));
            ExecutingOrders = executingOrders ?? throw new ArgumentNullException(nameof(executingOrders));
            CompleteOrders = completeOrders ?? throw new ArgumentNullException(nameof(completeOrders));
        }

        public OrderBookState Update(IEvent evnt)
        {
            if (evnt is NewOrderEvent newOrderEvent)
            {
                return new OrderBookState(
                    OpenOrders.Add(newOrderEvent.Order),
                    ExecutingOrders,
                    CompleteOrders);
            }

            if (evnt is AmendOrderEvent amendOrderEvent)
            {
                return new OrderBookState(
                    OpenOrders
                        .RemoveAll(o => o.OrderId == amendOrderEvent.Order.OrderId)
                        .Add(amendOrderEvent.Order),
                    ExecutingOrders,
                    CompleteOrders);
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
                            ExecutingOrders
                                .Add(m.Bid)
                                .Add(m.Ask),
                            CompleteOrders);
                    });
            }

            if (evnt is CompleteOrderEvent completeOrderEvent)
            {
                return new OrderBookState(
                    OpenOrders,
                    ExecutingOrders.RemoveAll(o => o.OrderId == completeOrderEvent.OrderId),
                    CompleteOrders.Add(
                        ExecutingOrders.Single(o => o.OrderId == completeOrderEvent.OrderId)));
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
