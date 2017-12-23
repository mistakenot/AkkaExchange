using System.Collections.Generic;
using AkkaExchange.Orders;

namespace AkkaExchange.Matching
{
    public class OrderMatchResult
    {
        public IEnumerable<OrderMatch> Matches { get; set; }
        public IEnumerable<Order> Orders { get; set; }

        public OrderMatchResult(
            IEnumerable<OrderMatch> matches, 
            IEnumerable<Order> orders)
        {
            Matches = matches;
            Orders = orders;
        }
    }
}