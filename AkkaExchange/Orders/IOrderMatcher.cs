using System.Collections.Generic;

namespace AkkaExchange.Orders
{
    public interface IOrderMatcher
    {
        OrderMatchResult Match(IEnumerable<PlacedOrder> orders);
    }
}