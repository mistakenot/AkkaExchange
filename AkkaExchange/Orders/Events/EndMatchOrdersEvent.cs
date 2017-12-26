using System;

namespace AkkaExchange.Orders.Events
{
    public class EndMatchOrdersEvent : IEvent
    {
        public OrderMatchResult MatchedOrders { get; }

        public EndMatchOrdersEvent(OrderMatchResult matchedOrders)
        {
            MatchedOrders = matchedOrders ?? throw new ArgumentNullException(nameof(matchedOrders));
        }
    }
}
