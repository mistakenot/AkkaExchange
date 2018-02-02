using System;

namespace AkkaExchange.Execution.Events
{
    public class UpdateOrderExecutionStatusEvent : Message, IEvent
    {
        public Guid OrderId { get; }
        public OrderExecutorStatus Status { get; }

        public UpdateOrderExecutionStatusEvent(Guid orderId, OrderExecutorStatus status)
        {
            OrderId = orderId;
            Status = status;
        }
    }
}