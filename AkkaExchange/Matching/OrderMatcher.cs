using System.Collections.Generic;
using System.Linq;
using AkkaExchange.Orders;
using AkkaExchange.Orders.Extensions;

namespace AkkaExchange.Matching
{
    public class OrderMatcher : IOrderMatcher
    {
        public OrderMatchResult Match(
            IEnumerable<Order> orders)
        {
            var bids = orders
                .Where(o => o.Side == OrderStateSide.Bid)
                .OrderByDescending(o => o.Price);

            var asks = orders
                .Where(o => o.Side == OrderStateSide.Ask)
                .OrderBy(o => o.Price);

            var matches = new List<OrderMatch>();

            var availableBids = new Stack<Order>(
                bids.Where(b => 
                    asks.Any(a => b.Price >= a.Price)));

            var availableAsks = new Stack<Order>(
                asks.Where(a => 
                    bids.Any(b => b.Price >= a.Price)));

            while (availableBids.Any())
            {
                var bid = availableBids.Pop();
                
                while (bid.Amount > 0 && availableAsks.Any() && bid.Amount >= availableAsks.Peek().Price)
                {
                    var ask = availableAsks.Pop();

                    var remainingAskAmount =
                        ask.Amount >= bid.Amount ? ask.Amount - bid.Amount : 0;
                    var remainingBuyAmount =
                        ask.Amount >= bid.Amount ? 0 : bid.Amount - ask.Amount;

                    var matchedBid = bid.WithAmount(bid.Amount - remainingBuyAmount);
                    var matchedAsk = ask.WithAmount(ask.Amount - remainingAskAmount);

                    matches.Add(new OrderMatch(matchedBid, matchedAsk));

                    bid = bid.WithAmount(remainingBuyAmount);

                    if (remainingAskAmount > 0)
                    {
                        availableAsks.Push(
                            ask.WithAmount(remainingAskAmount));
                    }
                    if (remainingBuyAmount > 0)
                    {
                        availableBids.Push(
                            bid.WithAmount(remainingBuyAmount));
                    }
                }
            }

            return new OrderMatchResult(
                matches, 
                availableAsks.Concat(availableBids));
        }
    }
}