using System;
using System.Linq;
using AkkaExchange.Orders;
using Xunit;

namespace AkkaExchange.Tests.Orders
{
    public class DefaultOrderMatcherTests
    {
        private readonly DefaultOrderMatcher _subject;

        public DefaultOrderMatcherTests()
        {
            _subject = new DefaultOrderMatcher();
        }

        [Fact]
        public void DefaultOrderMatcher_SamePriceSameSizeOrders_MatchOk()
        {
            var orders = new[]
            {
                Ask(1, 1),
                Bid(1, 1)
            };

            var result = _subject.Match(orders);

            var match = Assert.Single(result.Matches);
            var ask = match.Ask;
            var bid = match.Bid;
            Assert.Equal(1m, ask.Amount);
            Assert.Equal(1m, ask.Price);
            Assert.Equal(1m, bid.Amount);
            Assert.Equal(1m, bid.Price);
        }

        [Fact]
        public void DefaultOrderMatcher_SamePriceLargeAskSmallBid_MatchOk()
        {
            var orders = new[]
            {
                Ask(1, 1),
                Bid(0.5m, 1)
            };

            var result = _subject.Match(orders);

            var match = Assert.Single(result.Matches);
            var ask = match.Ask;
            var bid = match.Bid;
            Assert.Equal(0.5m, ask.Amount);
            Assert.Equal(1m, ask.Price);
            Assert.Equal(0.5m, bid.Amount);
            Assert.Equal(1m, bid.Price);
        }

        [Fact]
        public void DefaultOrderMatcher_SamePriceLargeBidSmallAsk_MatchOk()
        {
            var orders = new[]
            {
                Bid(1, 1),
                Ask(0.5m, 1)
            };

            var result = _subject.Match(orders);

            var match = Assert.Single(result.Matches);
            var ask = match.Ask;
            var bid = match.Bid;
            Assert.Equal(0.5m, ask.Amount);
            Assert.Equal(1m, ask.Price);
            Assert.Equal(0.5m, bid.Amount);
            Assert.Equal(1m, bid.Price);
        }

        [Fact]
        public void DefaultOrderMatcher_EqualSizeHighBidLowAsk_MatchOk()
        {
            var orders = new[]
            {
                Bid(1m, 2m),
                Ask(1m, 0.5m)
            };

            var result = _subject.Match(orders);

            var match = Assert.Single(result.Matches);
            var ask = match.Ask;
            var bid = match.Bid;
            Assert.Equal(1m, ask.Amount);
            Assert.Equal(1.25m, ask.Price);
            Assert.Equal(1m, bid.Amount);
            Assert.Equal(1.25m, bid.Price);
        }

        [Fact]
        public void DefaultOrderMatcher_EqualSizeLowBidHighAsk_MatchOk()
        {
            var orders = new[]
            {
                Bid(1, 0.5m),
                Ask(1, 2m)
            };

            var result = _subject.Match(orders);

            Assert.Empty(result.Matches);
        }

        [Fact]
        public void DefaultOrderMatcher_MultipleEqualSizeEqualPrice_MatchOk()
        {
            var orders = new[]
            {
                Bid(1, 1m),
                Ask(1, 1m),
                Bid(1, 1m),
                Ask(1, 1m)
            };

            var result = _subject.Match(orders);

            Assert.Equal(2, result.Matches.Count());
            Assert.All(result.Matches, m =>
            {
                var ask = m.Ask;
                var bid = m.Bid;
                Assert.Equal(1m, ask.Price);
                Assert.Equal(1m, ask.Amount);
                Assert.Equal(1m, bid.Amount);
                Assert.Equal(1m, bid.Price);
            });
        }

        [Fact]
        public void DefaultOrderMatcher_MultipleSmallBidLargeAskSamePrice_MatchOk()
        {
            var firstAsk =
                new PlacedOrder(
                    new Order(Guid.NewGuid(), 1, 1m, OrderSide.Ask),
                    DateTime.UtcNow.AddSeconds(-1));

            var orders = new[]
            {
                Bid(0.5m, 1m),
                firstAsk,
                Bid(0.5m, 1m),
                Ask(1, 1)
            };

            var result = _subject.Match(orders);

            Assert.Equal(2, result.Matches.Count());

            Assert.All(result.Matches, m =>
            {
                var ask = m.Ask;
                var bid = m.Bid;
                Assert.Equal(1m, ask.Price);
                Assert.Equal(0.5m, ask.Amount);
                Assert.Equal(1m, bid.Price);
                Assert.Equal(0.5m, bid.Amount);
                Assert.Equal(firstAsk.ClientId, ask.ClientId);
            });
        }

        [Fact]
        public void DefaultOrderMatcher_MultipleLargeBidSmallAskSamePrice_MatchOk()
        {
            var firstBid =
                new PlacedOrder(
                    new Order(Guid.NewGuid(), 1, 1m, OrderSide.Bid), DateTime.UtcNow.AddSeconds(-1));

            var orders = new[]
            {
                firstBid,
                Ask(0.5m, 1m),
                Bid(1m, 1m),
                Ask(0.5m, 1m)
            };

            var result = _subject.Match(orders);

            Assert.Equal(2, result.Matches.Count());

            Assert.All(result.Matches, m =>
            {
                var ask = m.Ask;
                var bid = m.Bid;
                Assert.Equal(1m, ask.Price);
                Assert.Equal(0.5m, ask.Amount);
                Assert.Equal(1m, bid.Price);
                Assert.Equal(0.5m, bid.Amount);
                Assert.Equal(firstBid.ClientId, bid.ClientId);
            });
        }

        [Fact]
        public void DefaultOrderMatcher_MultipleSameSizeDifferentPrice_MatchOk()
        {
            var orders = new[]
            {
                Bid(1m, 1m),
                Ask(1m, 0.5m),
                Bid(1m, 0.5m),
                Ask(1m, 1m)
            };

            var result = _subject.Match(orders);

            Assert.Equal(2, result.Matches.Count());
            Assert.All(result.Matches, r =>
            {
                Assert.Equal(0.75m, r.Ask.Price);
                Assert.Equal(0.75m, r.Bid.Price);
                Assert.Equal(1m, r.Ask.Amount);
                Assert.Equal(1m, r.Bid.Amount);
            });
        }

        [Fact]
        public void DefaultOrderMatcher_MultipleDifferentSizeSamePrice_MatchOk()
        {
            var orders = new[]
            {
                Bid(1m, 1m),
                Ask(0.5m, 1m),
                Bid(0.5m, 1m),
                Ask(1m, 1m)
            };

            var result = _subject.Match(orders);

            Assert.Equal(3, result.Matches.Count());
            Assert.All(result.Matches, r =>
            {
                Assert.Equal(1m, r.Ask.Price);
                Assert.Equal(1m, r.Bid.Price);
                Assert.Equal(0.5m, r.Ask.Amount);
                Assert.Equal(0.5m, r.Bid.Amount);
            });
        }

        private static PlacedOrder Ask(decimal amount, decimal price)
            => new PlacedOrder(
                new Order(Guid.NewGuid(), amount, price, OrderSide.Ask));

        private static PlacedOrder Bid(decimal amount, decimal price)
            => new PlacedOrder(
                new Order(Guid.NewGuid(), amount, price, OrderSide.Bid));
    }
}