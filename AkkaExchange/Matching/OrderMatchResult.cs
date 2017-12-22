using System.Collections.Generic;
using AkkaExchange.Orders;

namespace AkkaExchange.Matching
{
    public class OrderMatchResult
    {
        public IEnumerable<OrderMatch> Matches { get; set; }
        public IEnumerable<OrderDetails> Orders { get; set; }
    }
}