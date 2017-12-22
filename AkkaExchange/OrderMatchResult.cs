using System.Collections.Generic;
using AkkaExchange.State;

namespace AkkaExchange
{
    public class OrderMatchResult
    {
        public IEnumerable<OrderMatch> Matches { get; set; }
        public IEnumerable<OrderDetails> Orders { get; set; }
    }
}