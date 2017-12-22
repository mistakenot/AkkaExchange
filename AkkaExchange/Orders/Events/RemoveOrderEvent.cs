using System;

namespace AkkaExchange.Orders.Events
{
    public class RemoveOrderEvent : IEvent
    {
        public Guid OrderId { get; }

        public RemoveOrderEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
