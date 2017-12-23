using AkkaExchange.Orders;
using System;
using System.Collections.Generic;

namespace AkkaExchange.Matching
{
    public class OrderMatch
    {
        public IEnumerable<Order> Bids { get; }
        public IEnumerable<Order> Asks { get; }

        public OrderMatch(
            IEnumerable<Order> bids, 
            IEnumerable<Order> asks)
        {
            Bids = bids ?? throw new ArgumentNullException(nameof(bids));
            Asks = asks ?? throw new ArgumentNullException(nameof(asks));
        }

        public OrderMatch(Order bid, Order ask)
            : this(new [] { bid }, new [] { ask })
        {
            
        }
    }
}