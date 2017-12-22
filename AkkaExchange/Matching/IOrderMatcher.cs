using System.Collections.Generic;
using AkkaExchange.Orders;

namespace AkkaExchange.Matching
{
    public interface IOrderMatcher
    {
        OrderMatchResult Match(IEnumerable<OrderDetails> orders);
    }
}