using System.Collections.Immutable;
using System.Linq;
using System;
using AkkaExchange.Orders;
using AkkaExchange.Orders.Events;
using AkkaExchange.Orders.Extensions;
using Xunit;

namespace AkkaExchange.Tests.Orders
{
    public class OrderBookStateTests : OrderFixture
    {
        [Fact]
        public void OrderBookState_MatchedOrdersEventWithSingleCompleteOrder_Ok()
        {
            var bid = Bid(1m, 1m);
            var ask = Ask(1m, 1m);

            var state = new OrderBookState(
                ImmutableList<PlacedOrder>.Empty.Add(bid).Add(ask),
                ImmutableList<PlacedOrder>.Empty,
                ImmutableList<PlacedOrder>.Empty);

            var result = state.Update(
                Match(bid, ask));

            Assert.Empty(result.OpenOrders);
            Assert.Equal(2, result.ExecutingOrders.Count);
            Assert.Equal(bid.OrderId, result.ExecutingOrders.Single(r => r.Side == OrderSide.Bid).OrderId);
        }

        [Fact]
        public void OrderBookState_MatchedOrdersEventWithSinglePartiallyMatchedOrder_Ok()
        {
            var bid = Bid(1m, 1m);
            var ask = Ask(1m, 1m);

            var state = new OrderBookState(
                ImmutableList<PlacedOrder>.Empty.Add(bid).Add(ask),
                ImmutableList<PlacedOrder>.Empty,
                ImmutableList<PlacedOrder>.Empty);

            var result = state
                .Update(
                    Match(
                        bid.WithAmount(0.5m), 
                        ask.WithAmount(0.5m)));

            Assert.Equal(2, result.ExecutingOrders.Count);
            Assert.Equal(2, result.OpenOrders.Count);
            Assert.All(result.ExecutingOrders, o => Assert.Equal(0.5m, o.Amount));
            Assert.All(result.OpenOrders, o => Assert.Equal(0.5m, o.Amount));
            Assert.Equal(result.ExecutingOrders.First().OrderId, result.OpenOrders.First().OrderId);
            Assert.Equal(result.ExecutingOrders.Skip(1).Single().OrderId, result.OpenOrders.Skip(1).Single().OrderId);
        }

        [Fact]
        public void OrderBookState_CompleteOrdersEvent_Ok()
        {
            var bid = Bid(1m, 1m);
            var ask = Ask(1m, 1m);

            var state = new OrderBookState(
                ImmutableList<PlacedOrder>.Empty,
                ImmutableList<PlacedOrder>.Empty.Add(bid).Add(ask),
                ImmutableList<PlacedOrder>.Empty);

            var result = state
                .Update(new CompleteOrderEvent(bid, ask));

            Assert.Empty(result.OpenOrders);
            Assert.Empty(result.ExecutingOrders);
            Assert.Equal(2, result.CompleteOrders.Count);
        }

        [Fact]
        public void OrderBookState_CompletesPartialOrders_Ok()
        {
            var bid = Bid(2m, 1m);
            var ask = Ask(1m, 1m);

            var state = new OrderBookState(
                ImmutableList<PlacedOrder>.Empty,
                ImmutableList<PlacedOrder>.Empty.Add(bid).Add(ask),
                ImmutableList<PlacedOrder>.Empty);

            var result = state
                .Update(
                    new CompleteOrderEvent(
                        bid.WithAmount(1m),
                        ask));
            
            Assert.Empty(result.ExecutingOrders);
            var bidResult = Assert.Single(result.OpenOrders) as PlacedOrder;

            Assert.Equal(1m, bidResult.Amount);
            Assert.Equal(bid.Price, bidResult.Price);
            Assert.Equal(bid.ClientId, bidResult.ClientId);
            Assert.Equal(bid.OrderId, bidResult.OrderId);
            Assert.Equal(bid.Side, bidResult.Side);
        }

        private static MatchedOrdersEvent Match(PlacedOrder bid, PlacedOrder ask) => 
            new MatchedOrdersEvent(
                new OrderMatchResult(
                    new[] { new OrderMatch(bid, ask) }));
    }
}