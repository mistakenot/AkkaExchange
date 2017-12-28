using System.Collections.Generic;
using System.Linq;
using AkkaExchange.Orders.Extensions;

namespace AkkaExchange.Orders
{
    public interface IOrderMatcher
    {
        OrderMatchResult Match(IEnumerable<PlacedOrder> orders);
    }

    public class DefaultOrderMatcher : IOrderMatcher
    {
        public OrderMatchResult Match(
            IEnumerable<PlacedOrder> orders)
        {
            var bids = orders
                .Where(o => o.Details.Side == OrderSide.Bid)
                .OrderByDescending(o => o.Details.Price);

            var asks = orders
                .Where(o => o.Details.Side == OrderSide.Ask)
                .OrderBy(o => o.Details.Price);

            var matches = new List<OrderMatch>();

            var availableBids = new Stack<PlacedOrder>(
                bids.Where(b =>
                    asks.Any(a => b.Details.Price >= a.Details.Price)));

            var availableAsks = new Stack<PlacedOrder>(
                asks.Where(a =>
                    bids.Any(b => b.Details.Price >= a.Details.Price)));

            while (availableBids.Any())
            {
                var bid = availableBids.Pop();

                while (bid.Details.Amount > 0 && availableAsks.Any() && bid.Details.Amount >= availableAsks.Peek().Details.Price)
                {
                    var ask = availableAsks.Pop();

                    var remainingAskAmount =
                        ask.Details.Amount >= bid.Details.Amount ? ask.Details.Amount - bid.Details.Amount : 0;
                    var remainingBuyAmount =
                        ask.Details.Amount >= bid.Details.Amount ? 0 : bid.Details.Amount - ask.Details.Amount;

                    var matchedBid = bid.Details.WithAmount(bid.Details.Amount - remainingBuyAmount);
                    var matchedAsk = ask.Details.WithAmount(ask.Details.Amount - remainingAskAmount);

                    matches.Add(new OrderMatch(matchedBid, matchedAsk));

                    bid = bid.Details.WithAmount(remainingBuyAmount);

                    if (remainingAskAmount > 0)
                    {
                        availableAsks.Push(
                            ask.Details.WithAmount(remainingAskAmount));
                    }
                    if (remainingBuyAmount > 0)
                    {
                        availableBids.Push(
                            bid.Details.WithAmount(remainingBuyAmount));
                    }
                }
            }

            return new OrderMatchResult(
                matches,
                availableAsks.Concat(availableBids));
        }
    }
}