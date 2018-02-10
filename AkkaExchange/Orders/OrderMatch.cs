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

            if (bid.Side != OrderSide.Bid)
            {
                throw new ArgumentException("Bid order must be OrderSide.Bid");
            }

            if (ask.Side != OrderSide.Ask)
            {
                throw new ArgumentException("Ask order must be OrderSide.Ask");
            }

            if (bid.Amount != ask.Amount)
            {
                throw new ArgumentException("Bid and ask amounts must match.");
            }

            if (bid.Price != ask.Price)
            {
                throw new ArgumentException("Bid and ask prices must match.");
            }
        }
    }
}