using System;

namespace AkkaExchange.Orders.Events
{
    public class MatchedOrdersEvent : Message, IEvent
    {
        public string MatcherType { get; }
        public OrderMatchResult MatchedOrders { get; }

        public MatchedOrdersEvent(OrderMatchResult matchedOrders, string matcherType)
        {
            MatcherType = matcherType ?? throw new ArgumentNullException(nameof(matcherType));
            MatchedOrders = matchedOrders ?? throw new ArgumentNullException(nameof(matchedOrders));
        }
    }
}
