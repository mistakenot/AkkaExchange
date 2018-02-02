using System;

namespace AkkaExchange.Orders.Events
{
    public class NewOrderEvent : Message, IEvent
    {
        public PlacedOrder Order { get; }

        public NewOrderEvent(PlacedOrder order)
        {
            Order = order ?? throw new ArgumentNullException(nameof(order));
        }
    }
}
