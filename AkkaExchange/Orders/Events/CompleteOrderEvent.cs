using System;

namespace AkkaExchange.Orders.Events
{
    public class CompleteOrderEvent : IEvent
    {
        public Guid OrderId { get; }

        public CompleteOrderEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}