using System.Collections.Generic;

namespace AkkaExchange.Matching
{
    public class OrderMatcherState
    {
        public IEnumerable<OrderMatch> MatchedOrders { get; }
    }
}
