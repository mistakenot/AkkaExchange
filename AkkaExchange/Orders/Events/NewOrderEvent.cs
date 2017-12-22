using System;

namespace AkkaExchange.Orders.Events
{
    public class NewOrderEvent : IEvent
    {
        public Order Order { get; }

        public NewOrderEvent(Order order)
        {
            Order = order ?? throw new ArgumentNullException(nameof(order));
        }
    }
}
