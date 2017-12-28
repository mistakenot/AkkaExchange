using System.Collections.Generic;

namespace AkkaExchange.Orders
{
    public class OrderMatchResult
    {
        public IEnumerable<OrderMatch> Matches { get; set; }

        public OrderMatchResult(IEnumerable<OrderMatch> matches)
        {
            Matches = matches;
        }
    }
}