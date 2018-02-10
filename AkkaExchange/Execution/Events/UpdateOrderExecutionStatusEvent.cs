using System;
using AkkaExchange.Orders;

namespace AkkaExchange.Execution.Events
{
    public class UpdateOrderExecutionStatusEvent : Message, IEvent
    {
        public Guid OrderId { get; }
        public OrderMatch Match { get; }
        public OrderExecutorStatus Status { get; }

        public UpdateOrderExecutionStatusEvent(
            Guid orderId, 
            OrderMatch orderMatch,
            OrderExecutorStatus status)
        {
            OrderId = orderId;
            Match = orderMatch;
            Status = status;
        }
    }
}