using System.Collections.Immutable;
using System.Linq;
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
        public void OrderBookState_CompleteOrdersCommand_Ok()
        {
            var bid = Bid(1m, 1m);

            var state = new OrderBookState(
                ImmutableList<PlacedOrder>.Empty,
                ImmutableList<PlacedOrder>.Empty.Add(bid),
                ImmutableList<PlacedOrder>.Empty);

            var result = state
                .Update(new CompleteOrderEvent(bid.OrderId));

            Assert.Empty(result.OpenOrders);
            Assert.Empty(result.ExecutingOrders);
            Assert.Single(result.CompleteOrders);
        }

        private static MatchedOrdersEvent Match(PlacedOrder bid, PlacedOrder ask) => 
            new MatchedOrdersEvent(
                new OrderMatchResult(
                    new[] { new OrderMatch(bid, ask) }),
                "");
    }
}