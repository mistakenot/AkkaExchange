using AkkaExchange.Orders;
using System.Collections.Generic;

namespace AkkaExchange.Matching
{
    public class OrderMatch
    {
        public IEnumerable<Order> Bids { get; }
        public IEnumerable<Order> Asks { get; }
    }
}