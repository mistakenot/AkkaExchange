using System;

namespace AkkaExchange.Orders.Events
{
    public class RemoveOrderEvent : Message, IEvent
    {
        public Guid OrderId { get; }

        public RemoveOrderEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
