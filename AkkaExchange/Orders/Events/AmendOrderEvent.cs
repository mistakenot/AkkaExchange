using System;

namespace AkkaExchange.Orders.Events
{
    public class AmendOrderEvent : IEvent
    {
        public Order Order { get; }

        public AmendOrderEvent(Order order)
        {
            Order = order ?? throw new ArgumentNullException(nameof(order));
        }
    }
}