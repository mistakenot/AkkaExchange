using System;

namespace AkkaExchange.Orders.Events
{
    public class AmendOrderEvent : Message, IEvent
    {
        public PlacedOrder Order { get; }

        public AmendOrderEvent(PlacedOrder order)
        {
            Order = order ?? throw new ArgumentNullException(nameof(order));
        }
    }
}