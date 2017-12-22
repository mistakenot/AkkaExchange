using System;

namespace AkkaExchange.Events
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
