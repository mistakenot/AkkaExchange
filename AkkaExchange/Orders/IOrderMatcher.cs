using System.Collections.Generic;
using System.Linq;
using AkkaExchange.Orders.Extensions;

namespace AkkaExchange.Orders
{
    public interface IOrderMatcher
    {
        OrderMatchResult Match(IEnumerable<PlacedOrder> orders);
    }

    /// <summary>
    /// Default order matching algorithm. 
    /// - Prioritises based on price then when received.
    /// - Partially fills all orders.
    /// - Price is the simple average of bid and ask.
    /// </summary>
    public class DefaultOrderMatcher : IOrderMatcher
    {
        public OrderMatchResult Match(
            IEnumerable<PlacedOrder> orders)
        {
            var bids = orders
                .Where(o => o.Side == OrderSide.Bid && o.Amount > 0m)
                .OrderByDescending(o => o.Price)
                .ThenBy(o => o.PlacedAt);

            var asks = orders
                .Where(o => o.Side == OrderSide.Ask && o.Amount > 0m)
                .OrderBy(o => o.Price)
                .ThenBy(o => o.PlacedAt);

            var matches = new List<OrderMatch>();

            var availableBids = new Stack<PlacedOrder>(
                bids.Where(b =>
                    asks.Any(a => b.Price >= a.Price)).Reverse());

            var availableAsks = new Stack<PlacedOrder>(
                asks.Where(a =>
                    bids.Any(b => b.Price >= a.Price)).Reverse());

            while (availableBids.Any() && availableAsks.Any() &&
                   availableBids.Peek().Amount > 0 && availableAsks.Peek().Amount > 0)
            {
                var bid = availableBids.Pop();
                var ask = availableAsks.Pop();

                var remainingAskAmount =
                    ask.Amount >= bid.Amount ? ask.Amount - bid.Amount : 0;
                var remainingBidAmount =
                    ask.Amount >= bid.Amount ? 0 : bid.Amount - ask.Amount;

                var price = (bid.Price + ask.Price) / 2M;

                var matchedBid = bid
                    .WithAmount(bid.Amount - remainingBidAmount)
                    .WithPrice(price);
                var matchedAsk = ask
                    .WithAmount(ask.Amount - remainingAskAmount)
                    .WithPrice(price);

                matches.Add(new OrderMatch(matchedBid, matchedAsk));

                if (remainingAskAmount > 0)
                {
                    availableAsks.Push(
                        new PlacedOrder(
                            ask.WithAmount(remainingAskAmount)));
                }
                if (remainingBidAmount > 0)
                {
                    availableBids.Push(
                        new PlacedOrder(
                            bid.WithAmount(remainingBidAmount)));
                }
            }

            return new OrderMatchResult(matches);
        }
    }
}