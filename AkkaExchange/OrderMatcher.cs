using System.Collections.Generic;
using System.Linq;
using AkkaExchange.Handlers;
using AkkaExchange.State;

namespace AkkaExchange
{
    public class OrderMatcher : IOrderMatcher
    {
        public OrderMatchResult Match(IEnumerable<OrderDetails> orders)
        {
            var bids = orders
                .Where(o => o.Side == OrderStateSide.Bid)
                .OrderByDescending(o => o.Price);

            var asks = orders
                .Where(o => o.Side == OrderStateSide.Ask)
                .OrderBy(o => o.Price);

            var matches = new List<OrderMatch>();

            var availableBids = new Stack<OrderDetails>(
                bids.Where(b => 
                    asks.All(a => b.Price >= a.Price)));

            while (availableBids.Any())
            {
                var bid = availableBids.Pop();

                var availableAsks = new Stack<OrderDetails>(
                    asks.Where(a => a.Price <= bid.Price));

                var remainingBuy = bid;

                while (remainingBuy.Amount > 0 && availableAsks.Any())
                {
                    var ask = availableAsks.Pop();

                    var remainingAskAmount =
                        ask.Amount >= remainingBuy.Amount ? ask.Amount - remainingBuy.Amount : 0;
                    var remainingBuyAmount =
                        ask.Amount >= remainingBuy.Amount ? 0 : remainingBuy.Amount - ask.Amount;

                    remainingBuy = new OrderDetails(remainingBuyAmount, remainingBuy.Price, OrderStateSide.Bid);

                    if (remainingAskAmount > 0)
                    {
                        availableAsks.Push(
                            ask.WithAmount(remainingAskAmount));
                    }
                    else
                    {
                        availableBids.Push(
                            bid.WithAmount(remainingBuyAmount));
                    }
                }
            }

            return null;
            
        }
    }
}