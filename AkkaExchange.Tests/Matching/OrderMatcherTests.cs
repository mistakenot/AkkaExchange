using System;
using System.Linq;
using AkkaExchange.Orders;
using Xunit;

namespace AkkaExchange.Tests.Matching
{
    public class OrderMatcherTests
    {
        [Fact]
        public void OrderMatcher_TwoEqualOrders_MatchOk()
        {
            var matcher = new DefaultOrderMatcher();
            var orders = new[]
            {
                new Order(Guid.NewGuid(), Guid.Empty, 1, 1, OrderSide.Ask),
                new Order(Guid.NewGuid(), Guid.Empty, 1, 1, OrderSide.Bid)
            };

            var result = matcher.Match(orders);

            Assert.Single(result.Matches);
            Assert.Empty(result.Orders);
        }

        // [Fact]
        public void OrderMatcher_TwoUnerualOrders_MatchOk()
        {
            var matcher =  new DefaultOrderMatcher();
            var orders = new[]
            {
                new Order(Guid.NewGuid(), Guid.Empty, 1.5m, 1, OrderSide.Ask),
                new Order(Guid.NewGuid(), Guid.Empty, 1, 1, OrderSide.Bid),
                new Order(Guid.NewGuid(), Guid.Empty, 1, 1, OrderSide.Bid)
            };

            var result = matcher.Match(orders);

            Assert.Equal(2, result.Matches.Count());
            Assert.Single(result.Orders);
        }
    }
}
