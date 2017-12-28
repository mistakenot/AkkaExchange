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
        [Fact(DisplayName = "OrderBookState receives MatchedOrdersEvent containing two, fully matched orders.")]
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
            Assert.Equal(2, result.PendingOrders.Count);
            Assert.Equal(bid.OrderId, result.PendingOrders.Single(r => r.Side == OrderSide.Bid).OrderId);
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

            Assert.Equal(2, result.PendingOrders.Count);
            Assert.Equal(2, result.OpenOrders.Count);
            Assert.All(result.PendingOrders, o => Assert.Equal(0.5m, o.Amount));
            Assert.All(result.OpenOrders, o => Assert.Equal(0.5m, o.Amount));
            Assert.Equal(result.PendingOrders.First().OrderId, result.OpenOrders.First().OrderId);
            Assert.Equal(result.PendingOrders.Skip(1).Single().OrderId, result.OpenOrders.Skip(1).Single().OrderId);
        }

        private static MatchedOrdersEvent Match(PlacedOrder bid, PlacedOrder ask) => 
            new MatchedOrdersEvent(
                new OrderMatchResult(
                    new[] { new OrderMatch(bid, ask) }),
                "");
    }
}