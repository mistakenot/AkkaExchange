using System;

namespace AkkaExchange.Orders.Events
{
    public class AmendOrderEvent : IEvent
    {
        public PlacedOrder Order { get; }

        public AmendOrderEvent(PlacedOrder order)
        {
            Order = order ?? throw new ArgumentNullException(nameof(order));
        }
    }
}