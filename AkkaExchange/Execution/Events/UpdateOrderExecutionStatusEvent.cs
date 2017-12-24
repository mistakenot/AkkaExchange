using System;

namespace AkkaExchange.Execution.Events
{
    public class UpdateOrderExecutionStatusEvent : IEvent
    {
        public Guid StateOrderId { get; }
        public OrderExecutorStatus CommandStatus { get; }

        public UpdateOrderExecutionStatusEvent(Guid stateOrderId, OrderExecutorStatus commandStatus)
        {
            StateOrderId = stateOrderId;
            CommandStatus = commandStatus;
        }
    }
}