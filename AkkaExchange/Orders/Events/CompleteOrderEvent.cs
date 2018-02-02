using System;

namespace AkkaExchange.Orders.Events
{
    public class CompleteOrderEvent : Message, IEvent
    {
        public Guid OrderId { get; }

        public CompleteOrderEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}