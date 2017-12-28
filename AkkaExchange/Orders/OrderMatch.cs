using System;

namespace AkkaExchange.Orders
{
    public class OrderMatch
    {
        public PlacedOrder Bid { get; }
        public PlacedOrder Ask { get; }

        public OrderMatch(PlacedOrder bid, PlacedOrder ask)
        {
            Ask = ask ?? throw new ArgumentNullException(nameof(ask));
            Bid = bid ?? throw new ArgumentNullException(nameof(bid));
        }
    }
}