using System.Collections.Generic;
using AkkaExchange.State;

namespace AkkaExchange.Handlers
{
    public interface IOrderMatcher
    {
        OrderMatchResult Match(IEnumerable<OrderDetails> orders);
    }
}